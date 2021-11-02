using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SampleOrderParameterExtensions
    {
        internal static IResult<SetSampleOrderParameters> ToParsedParameters(this ISetSampleOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            SampleOrderKey sampleOrderKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.SampleOrderKey))
            {
                var sampleOrderKeyResult = KeyParserHelper.ParseResult<ISampleOrderKey>(parameters.SampleOrderKey);
                if(!sampleOrderKeyResult.Success)
                {
                    return sampleOrderKeyResult.ConvertTo<SetSampleOrderParameters>();
                }

                sampleOrderKey = sampleOrderKeyResult.ResultingObject.ToSampleOrderKey();
            }

            CustomerKey requestCustomerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.RequestedByCompanyKey))
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.RequestedByCompanyKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<SetSampleOrderParameters>();
                }

                requestCustomerKey = customerKeyResult.ResultingObject.ToCustomerKey();
            }

            CompanyKey brokerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.BrokerKey))
            {
                var companyKey = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!companyKey.Success)
                {
                    return companyKey.ConvertTo<SetSampleOrderParameters>();
                }

                brokerKey = companyKey.ResultingObject.ToCompanyKey();
            }

            var itemParameters = parameters.Items.ToParsedParameters(ToParsedParameters);
            if(!itemParameters.Success)
            {
                return itemParameters.ConvertTo<SetSampleOrderParameters>();
            }
            
            return new SuccessResult<SetSampleOrderParameters>(new SetSampleOrderParameters
                {
                    Parameters = parameters,
                    SampleOrderKey = sampleOrderKey,
                    RequestCustomerKey = requestCustomerKey,
                    BrokerKey = brokerKey,

                    Items = itemParameters.ResultingObject
                });
        }

        internal static IResult<SetSampleOrderItemParameters> ToParsedParameters(this ISampleOrderItemParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            SampleOrderItemKey itemKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.SampleOrderItemKey))
            {
                var itemKeyResult = KeyParserHelper.ParseResult<ISampleOrderItemKey>(parameters.SampleOrderItemKey);
                if(!itemKeyResult.Success)
                {
                    return itemKeyResult.ConvertTo<SetSampleOrderItemParameters>();
                }

                itemKey = itemKeyResult.ResultingObject.ToSampleOrderItemKey();
            }

            ProductKey productKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.ProductKey))
            {
                var chileProductKeyResult = KeyParserHelper.ParseResult<IProductKey>(parameters.ProductKey);
                if(!chileProductKeyResult.Success)
                {
                    return chileProductKeyResult.ConvertTo<SetSampleOrderItemParameters>();
                }

                productKey = chileProductKeyResult.ResultingObject.ToProductKey();
            }

            LotKey lotKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.LotKey))
            {
                var chileLotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
                if(!chileLotKeyResult.Success)
                {
                    return chileLotKeyResult.ConvertTo<SetSampleOrderItemParameters>();
                }

                lotKey = chileLotKeyResult.ResultingObject.ToLotKey();
            }

            return new SuccessResult<SetSampleOrderItemParameters>(new SetSampleOrderItemParameters
                {
                    Parameters = parameters,

                    SampleOrderItemKey = itemKey,
                    ProductKey = productKey,
                    LotKey = lotKey
                });
        }
    }
}