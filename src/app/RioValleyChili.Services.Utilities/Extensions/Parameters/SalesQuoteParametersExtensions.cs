using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SalesQuoteParametersExtensions
    {
        internal static IResult<SalesQuoteParameters> ToParsedParameters(this ISalesQuoteParameters parameters, bool updateExisting)
        {
            if(updateExisting && parameters.SalesQuoteNumber == null)
            {
                return new InvalidResult<SalesQuoteParameters>(null, UserMessages.SalesQuoteNumberRequired);
            }

            FacilityKey sourceFacilityKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.SourceFacilityKey))
            {
                var sourceFacilityKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.SourceFacilityKey);
                if(!sourceFacilityKeyResult.Success)
                {
                    return sourceFacilityKeyResult.ConvertTo<SalesQuoteParameters>();
                }
                sourceFacilityKey = sourceFacilityKeyResult.ResultingObject.ToFacilityKey();
            }
            
            CustomerKey customerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.CustomerKey))
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<SalesQuoteParameters>();
                }
                customerKey = customerKeyResult.ResultingObject.ToCustomerKey();
            }

            CompanyKey brokerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.BrokerKey))
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<SalesQuoteParameters>();
                }
                brokerKey = brokerKeyResult.ResultingObject.ToCompanyKey();
            }

            var itemParameters = new List<SalesQuoteItemParameters>();
            foreach(var item in parameters.Items ?? new ISalesQuoteItemParameters[0])
            {
                SalesQuoteItemKey itemKey = null;
                if(updateExisting && !string.IsNullOrWhiteSpace(item.SalesQuoteItemKey))
                {
                    var itemKeyResult = KeyParserHelper.ParseResult<ISalesQuoteItemKey>(item.SalesQuoteItemKey);
                    if(!itemKeyResult.Success)
                    {
                        return itemKeyResult.ConvertTo<SalesQuoteParameters>();
                    }
                    itemKey = itemKeyResult.ResultingObject.ToSalesQuoteItemKey();
                }

                var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(item.ProductKey);
                if(!productKeyResult.Success)
                {
                    return productKeyResult.ConvertTo<SalesQuoteParameters>();
                }

                var packagingKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(item.PackagingKey);
                if(!packagingKeyResult.Success)
                {
                    return packagingKeyResult.ConvertTo<SalesQuoteParameters>();
                }

                var treatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(item.TreatmentKey);
                if(!treatmentKeyResult.Success)
                {
                    return treatmentKeyResult.ConvertTo<SalesQuoteParameters>();
                }

                itemParameters.Add(new SalesQuoteItemParameters
                    {
                        Parameters = item,
                        SalesQuoteItemKey = itemKey,
                        ProductKey = productKeyResult.ResultingObject.ToProductKey(),
                        PackagingProductKey = packagingKeyResult.ResultingObject.ToPackagingProductKey(),
                        InventoryTreatmentKey = treatmentKeyResult.ResultingObject.ToInventoryTreatmentKey()
                    });
            }

            return new SuccessResult<SalesQuoteParameters>(new SalesQuoteParameters
                {
                    Parameters = parameters,
                    SalesQuoteNumber = updateExisting ? parameters.SalesQuoteNumber : null,
                    SourceFacilityKey = sourceFacilityKey,
                    CustomerKey = customerKey,
                    BrokerKey = brokerKey,
                    Items = itemParameters
                });
        }
    }
}