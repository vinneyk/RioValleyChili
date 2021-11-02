// ReSharper disable CompareOfFloatsByEqualityOperator

using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class CalculateAttributeWeightedAveragesCommand
    {
        private readonly ILotUnitOfWork _lotUnifOfWork;

        internal CalculateAttributeWeightedAveragesCommand(ILotUnitOfWork lotUnifOfWork)
        {
            if(lotUnifOfWork == null) { throw new ArgumentNullException("lotUnifOfWork"); }
            _lotUnifOfWork = lotUnifOfWork;
        }

        internal IResult<Dictionary<string, double>> Execute(Dictionary<IInventoryKey, int> inventoryKeysAndQuantities)
        {
            if(inventoryKeysAndQuantities == null) { throw new ArgumentNullException("inventoryKeysAndQuantities"); }
            if(inventoryKeysAndQuantities.Count <= 0) { return new InvalidResult<Dictionary<string, double>>(null, UserMessages.EmptySet); }
            
            var keysAndQuantities = inventoryKeysAndQuantities.Select(i => new LotPackagingKeyValueQuantity(i.Key, i.Key, i.Value)).ToList();

            var lotPredicate = PredicateHelper.OrPredicates(keysAndQuantities.DistinctBySelect(k => k.LotKeyValue, k => k.LotPredicateExpression));
            var lots = _lotUnifOfWork.LotRepository.Filter(lotPredicate, l => l.Attributes.Select(a => a.AttributeName)).ToList();

            var packagingPredicate = PredicateHelper.OrPredicates(keysAndQuantities.DistinctBySelect(k => k.PackagingKeyValue, k => k.PackagingPredicateExpression));
            var packagingProducts = _lotUnifOfWork.PackagingProductRepository.Filter(packagingPredicate).ToList();

            List<IWeightedLotAttributes> attributesAndWeights;
            try
            {
                attributesAndWeights = keysAndQuantities.Select(k => (IWeightedLotAttributes) new LotAttributesAndWeight(k, lots, packagingProducts)).ToList();
            }
            catch(Exception exception)
            {
                if(exception is ObjectNotFoundException)
                {
                    return new FailureResult<Dictionary<string, double>>(null, exception.Message);
                }
                throw;
            }
            
            var weightedAveragesResult = CalculateWeightedAverages(attributesAndWeights);
            if(!weightedAveragesResult.Success)
            {
                return weightedAveragesResult.ConvertTo<Dictionary<string, double>>();
            }

            return new SuccessResult<Dictionary<string, double>>(weightedAveragesResult.ResultingObject.Where(a => a.Key.Attribute.NameActive)
                .ToDictionary(a => a.Key.Attribute.Name, a => a.Value));
        }

        public static IResult<Dictionary<AttributeNameAndKey, double>> Execute(IEnumerable<PickedInventoryItem> pickedInventoryItems)
        {
            return CalculateWeightedAverages(pickedInventoryItems.Select(i => new LotAttributesAndWeight(i.Lot, i.PackagingProduct, i.Quantity)));
        }

        public static IResult<Dictionary<AttributeNameAndKey, double>> CalculateWeightedAverages(IEnumerable<IPickedInventoryItemReturn> items)
        {
            return CalculateWeightedAverages(items.Select(i => new LotAttributesAndWeight(i)));
        }

        public static IResult<Dictionary<AttributeNameAndKey, double>> CalculateWeightedAverages(IEnumerable<IWeightedLotAttributes> lotAttributes)
        {
            var totalWeight = 0.0;
            var weightedAttributes = lotAttributes.SelectMany(l =>
                {
                    totalWeight += l.LotWeight;
                    return l.LotAttributes.Select(a => new
                        {
                            Name = a.AttributeNameKey_ShortName,
                            WeightValue = a.Value * l.LotWeight
                        });
                })
                .ToList()
                .GroupBy(a => a.Name)
                .ToDictionary(a => a.Key, a => totalWeight == 0.0 ? 0.0 : a.Sum(w => w.WeightValue) / totalWeight);

            var results = StaticAttributeNames.AttributeNames.Select(a =>
                {
                    double value;
                    return new AttributeNameAndKey
                        {
                            Attribute = new LotAttributeSelect
                                {
                                    AttributeNameKey_ShortName = a.ShortName,
                                    Name = a.Name,
                                    NameActive = a.Active,
                                    Value = weightedAttributes.TryGetValue(a.ShortName, out value) ? value : 0.0
                                },
                            AttributeNameKey = new AttributeNameKey(a)
                        };
                })
                .ToDictionary(a => a, a => a.Attribute.Value);
            return new SuccessResult<Dictionary<AttributeNameAndKey, double>>(results);
        }

        private class LotPackagingKeyValueQuantity
        {
            internal string LotKeyValue { get; private set; }
            internal Expression<Func<Lot, bool>> LotPredicateExpression { get; private set; }
            internal Func<Lot, bool> LotPredicate { get; private set; }

            internal string PackagingKeyValue { get; private set; }
            internal Expression<Func<PackagingProduct, bool>> PackagingPredicateExpression { get; private set; }
            internal Func<PackagingProduct, bool> PackagingPredicate { get; private set; }

            internal int Quantity { get; private set; }

            internal LotPackagingKeyValueQuantity(ILotKey lot, IPackagingProductKey packaging, int quantity)
            {
                var lotKey = new LotKey(lot);
                LotKeyValue = lotKey.KeyValue;
                LotPredicateExpression = lotKey.FindByPredicate;
                LotPredicate = lotKey.FindByPredicate.Compile();

                var packagingKey = new PackagingProductKey(packaging);
                PackagingKeyValue = packagingKey.KeyValue;
                PackagingPredicateExpression = packagingKey.FindByPredicate;
                PackagingPredicate = packagingKey.FindByPredicate.Compile();

                Quantity = quantity;
            }
        }

        private class LotAttributesAndWeight : IWeightedLotAttributes
        {
            public List<ILotAttributeParameter> LotAttributes { get; private set; }
            public double LotWeight { get; private set; }

            internal LotAttributesAndWeight(LotPackagingKeyValueQuantity lotPackagingKeyValueQuantity, IEnumerable<Lot> lots, IEnumerable<PackagingProduct> packagingProducts)
            {
                var lot = lots.FirstOrDefault(lotPackagingKeyValueQuantity.LotPredicate);
                if(lot == null)
                {
                    throw new ObjectNotFoundException(string.Format(UserMessages.LotNotFound, lotPackagingKeyValueQuantity.LotKeyValue));
                }

                var packaging = packagingProducts.FirstOrDefault(lotPackagingKeyValueQuantity.PackagingPredicate);
                if(packaging == null)
                {
                    throw new ObjectNotFoundException(string.Format(UserMessages.PackagingProductNotFound, lotPackagingKeyValueQuantity.PackagingKeyValue));
                }

                LotAttributes = lot.Attributes.Select(a => (ILotAttributeParameter)new LotAttributeSelect
                    {
                        AttributeNameKey_ShortName = a.AttributeShortName,
                        Name = a.AttributeName.Name,
                        NameActive = a.AttributeName.Active,
                        Value = a.AttributeValue
                    }).ToList();
                LotWeight = packaging.Weight * lotPackagingKeyValueQuantity.Quantity;
            }

            internal LotAttributesAndWeight(Lot lot, PackagingProduct packagingProduct, int quantity)
            {
                LotAttributes = lot.Attributes.Select(a => new LotAttributeSelect
                    {
                        AttributeNameKey_ShortName = a.AttributeShortName,
                        Name = a.AttributeName.Name,
                        NameActive = a.AttributeName.Active,
                        Value = a.AttributeValue
                    })
                    .Cast<ILotAttributeParameter>()
                    .ToList();
                LotWeight = packagingProduct.Weight * quantity;
            }

            internal LotAttributesAndWeight(IPickedInventoryItemReturn item)
            {
                LotAttributes = item.Attributes.Select(a => new LotAttributeSelect
                    {
                        AttributeNameKey_ShortName = a.Key,
                        Name = a.Name,
                        NameActive = true,
                        Value = a.Value
                    })
                    .Cast<ILotAttributeParameter>()
                    .ToList();
                LotWeight = item.PackagingProduct.Weight * item.QuantityPicked;
            }
        }

        private class LotAttributeSelect : ILotAttributeParameter
        {
            public string AttributeNameKey_ShortName { get; internal set; }
            public string Name { get; internal set; }
            public bool NameActive { get; internal set; }
            public double Value { get; internal set; }
        }

        public class AttributeNameAndKey
        {
            public ILotAttributeParameter Attribute { get; set; }
            public AttributeNameKey AttributeNameKey { get; set; }
        }
    }
}

// ReSharper restore CompareOfFloatsByEqualityOperator