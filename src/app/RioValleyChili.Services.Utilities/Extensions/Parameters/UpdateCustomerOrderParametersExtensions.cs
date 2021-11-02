using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class UpdateCustomerOrderParametersExtensions
    {
        internal static IResult<UpdateSalesOrderCommandParameters> ToParsedParameters(this IUpdateSalesOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var customerOrderKeyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(parameters.SalesOrderKey);
            if(!customerOrderKeyResult.Success)
            {
                return customerOrderKeyResult.ConvertTo<UpdateSalesOrderCommandParameters>();
            }

            var facilityKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.FacilitySourceKey);
            if(!facilityKeyResult.Success)
            {
                return facilityKeyResult.ConvertTo<UpdateSalesOrderCommandParameters>();
            }

            ICompanyKey brokerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.BrokerKey))
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<UpdateSalesOrderCommandParameters>();
                }
                brokerKey = brokerKeyResult.ResultingObject;
            }

            var orderItemsResult = parameters.PickOrderItems == null ? null : parameters.PickOrderItems.ToParsedParametersList();
            if(orderItemsResult != null && !orderItemsResult.Success)
            {
                return orderItemsResult.ConvertTo<UpdateSalesOrderCommandParameters>();
            }

            return new SuccessResult().ConvertTo(new UpdateSalesOrderCommandParameters
                {
                    UpdateParameters = parameters,
                    SalesOrderKey = customerOrderKeyResult.ResultingObject.ToSalesOrderKey(),
                    BrokerKey = brokerKey == null ? null : brokerKey.ToCompanyKey(),
                    ShipFromFacilityKey = facilityKeyResult.ResultingObject.ToFacilityKey(),
                    OrderItems = orderItemsResult == null ? null : orderItemsResult.ResultingObject
                });
        }
    }
}