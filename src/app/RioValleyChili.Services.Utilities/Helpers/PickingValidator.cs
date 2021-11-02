using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal class PickingValidator
    {
        public PickingValidator(PickableInventoryItemReturn inventory)
        {
            _lotKey = inventory.LotKey;
            _lotQualityStatus = inventory.QualityStatus;
            _contractAllowances = inventory.ContractAllowances == null ? new List<string>() : inventory.ContractAllowances.Select(a => a.ContractKey).ToList();
            _customerOrderAllowances = inventory.CustomerOrderAllowances == null ? new List<string>() : inventory.CustomerOrderAllowances.Select(a => a.CustomerOrderKey).ToList();
            _customerAllowances = inventory.CustomerAllowances == null ? new List<string>() : inventory.CustomerAllowances.Select(a => a.CustomerKey).ToList();
            _attributeAndDefects = CompileAttributesAndDefects(inventory);
        }

        public PickingValidator(Lot lot)
        {
            _lotKey = lot.ToLotKey();
            _lotQualityStatus = lot.QualityStatus;
            _contractAllowances = lot.ContractAllowances.Select(a => a.ToContractKey().KeyValue).ToList();
            _customerOrderAllowances = lot.SalesOrderAllowances.Select(a => a.ToSalesOrderKey().KeyValue).ToList();
            _customerAllowances = lot.CustomerAllowances.Select(a => a.ToCustomerKey().KeyValue).ToList();
            _attributeAndDefects = CompileAttributesAndDefects(lot.Attributes, lot.AttributeDefects);
        }

        public IResult ValidForPicking(PickingValidatorContext context)
        {
            if(!string.IsNullOrWhiteSpace(context.ContractKey) && _contractAllowances.Contains(context.ContractKey))
            {
                return new SuccessResult();
            }

            if(!string.IsNullOrWhiteSpace(context.CustomerOrderKey) && _customerOrderAllowances.Contains(context.CustomerOrderKey))
            {
                return new SuccessResult();
            }

            if(!string.IsNullOrWhiteSpace(context.CustomerKey) && _customerAllowances.Contains(context.CustomerKey))
            {
                return new SuccessResult();
            }

            var invalidResults = context.AttributeSpecs
                .Select(s => s.Value.IsValidForPicking(_attributeAndDefects, s.Key, _lotKey, _lotQualityStatus))
                .Where(r => !r.Success)
                .ToList();
            if(invalidResults.Any())
            {
                return new InvalidResult(invalidResults.CombineMessages());
            }

            return new SuccessResult();
        }

        private static Dictionary<string, PickingValidatorAttributeAndDefects> CompileAttributesAndDefects(ILotSummaryReturn inventory)
        {
            var attributeDefects = inventory.Defects.Where(d => d.AttributeDefect != null)
                .GroupBy(d => d.AttributeDefect.AttributeShortName)
                .ToDictionary(g => g.Key, g => g.ToList());

            var results = new Dictionary<string, PickingValidatorAttributeAndDefects>();
            foreach(var attribute in inventory.Attributes)
            {
                List<ILotDefectReturn> defects;
                if(!attributeDefects.TryGetValue(attribute.Key, out defects))
                {
                    defects = new List<ILotDefectReturn>();
                }

                results.Add(attribute.Key, new PickingValidatorAttributeAndDefects(attribute, defects));
            }

            return results;
        }

        private static Dictionary<string, PickingValidatorAttributeAndDefects> CompileAttributesAndDefects(IEnumerable<LotAttribute> attributes, IEnumerable<LotAttributeDefect> lotAttributeDefects)
        {
            var attributeDefects = lotAttributeDefects
                .GroupBy(d => d.ToAttributeNameKey().KeyValue)
                .ToDictionary(g => g.Key, g => g.ToList());

            var results = new Dictionary<string, PickingValidatorAttributeAndDefects>();
            foreach(var attribute in attributes)
            {
                List<LotAttributeDefect> defects;
                var attributeNameKey = attribute.ToAttributeNameKey();
                if(!attributeDefects.TryGetValue(attributeNameKey, out defects))
                {
                    defects = new List<LotAttributeDefect>();
                }

                results.Add(attributeNameKey, new PickingValidatorAttributeAndDefects(attribute, defects));
            }

            return results;
        }

        private readonly string _lotKey;
        private readonly LotQualityStatus _lotQualityStatus;
        private readonly List<string> _contractAllowances;
        private readonly List<string> _customerOrderAllowances;
        private readonly List<string> _customerAllowances;
        private readonly Dictionary<string, PickingValidatorAttributeAndDefects> _attributeAndDefects;
    }

    internal class PickingValidatorContext
    {
        public string ContractKey, CustomerOrderKey, CustomerKey;
        public readonly Dictionary<string, PickingValidatorAttributeSpec> AttributeSpecs = new Dictionary<string, PickingValidatorAttributeSpec>();

        public PickingValidatorContext(IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec,
                                IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec,
                                IContractKey contractKey, ISalesOrderKey salesOrderKey, ICustomerKey customerKey)
        {
            if(contractKey != null)
            {
                ContractKey = contractKey.ToContractKey();
            }

            if(salesOrderKey != null)
            {
                CustomerOrderKey = salesOrderKey.ToSalesOrderKey();
            }

            if(customerKey != null)
            {
                CustomerKey = customerKey.ToCustomerKey();
            }

            var productSpecRanges = productSpec == null ? new Dictionary<string, IAttributeRange>() : productSpec.To().Dictionary<string, IAttributeRange>(p => p.Key, p => p.Value);
            var customerSpecRanges = customerSpec == null ? new Dictionary<string, IAttributeRange>() : customerSpec.To().Dictionary<string, IAttributeRange>(p => p.Key, p => p.Value);
            foreach(var prodSpec in productSpecRanges)
            {
                IAttributeRange custRange;
                if(customerSpecRanges.TryGetValue(prodSpec.Key, out custRange))
                {
                    customerSpecRanges.Remove(prodSpec.Key);
                }

                AttributeSpecs.Add(prodSpec.Key, new PickingValidatorAttributeSpec(prodSpec.Value, custRange));
            }

            foreach(var custSpec in customerSpecRanges)
            {
                AttributeSpecs.Add(custSpec.Key, new PickingValidatorAttributeSpec(null, custSpec.Value));
            }
        }
    }

    internal class PickingValidatorAttributeSpec
    {
        public PickingValidatorAttributeSpec(IAttributeRange productSpec, IAttributeRange customerSpec)
        {
            _productSpec = productSpec;
            _customerSpec = customerSpec;

            if(_productSpec != null)
            {
                if(_customerSpec == null)
                {
                    _looseProductSpec = true;
                }
                else
                {
                    _looseProductSpec = _productSpec.RangeMin >= _customerSpec.RangeMin && _productSpec.RangeMax <= _customerSpec.RangeMax;
                }
            }
        }

        public IResult IsValidForPicking(IDictionary<string, PickingValidatorAttributeAndDefects> attributesAndDefects, string attributeKey, string lotKey, LotQualityStatus qualityStatus)
        {
            var applicableSpec = _customerSpec != null ? UserMessages.CustomerSpec : UserMessages.ProductSpec;
            PickingValidatorAttributeAndDefects attributeAndDefects;
            if(!attributesAndDefects.TryGetValue(attributeKey, out attributeAndDefects))
            {
                if(_looseProductSpec && qualityStatus == LotQualityStatus.Released)
                {
                    return new SuccessResult();
                }

                return new InvalidResult(string.Format(UserMessages.LotMissingRequiredAttribute, lotKey, attributeKey, applicableSpec));
            }

            if(_customerSpec != null ? _customerSpec.ValueOutOfRange(attributeAndDefects.AttributeValue) : _productSpec.ValueOutOfRange(attributeAndDefects.AttributeValue))
            {
                if(!_looseProductSpec || !attributeAndDefects.DefectResolution.Any() || attributeAndDefects.DefectResolution.Any(d => !d))
                {
                    return new InvalidResult(string.Format(UserMessages.LotAttributeOutOfRequiredRange, lotKey, attributeKey, applicableSpec));
                }
            }
            return new SuccessResult();
        }

        private readonly IAttributeRange _productSpec;
        private readonly IAttributeRange _customerSpec;
        private readonly bool _looseProductSpec;
    }

    internal class PickingValidatorAttributeAndDefects
    {
        public readonly double AttributeValue;
        public readonly List<bool> DefectResolution;

        public PickingValidatorAttributeAndDefects(ILotAttributeReturn attribute, IEnumerable<ILotDefectReturn> defects)
        {
            AttributeValue = attribute.Value;
            DefectResolution = defects.Select(d => d.Resolution != null).ToList();
        }

        public PickingValidatorAttributeAndDefects(LotAttribute attribute, IEnumerable<LotAttributeDefect> defects)
        {
            AttributeValue = attribute.AttributeValue;
            DefectResolution = defects.Select(d => d.LotDefect.Resolution != null).ToList();
        }
    }
}