using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateCustomerOrderParametersExtensions
    {
        internal static IResult<CreateSalesOrderConductorParameters> ToParsedParameters(this ICreateSalesOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            ICustomerKey customerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.CustomerKey))
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<CreateSalesOrderConductorParameters>();
                }
                customerKey = customerKeyResult.ResultingObject;
            }

            ICompanyKey brokerKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.BrokerKey))
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<CreateSalesOrderConductorParameters>();
                }
                brokerKey = brokerKeyResult.ResultingObject;
            }

            var facilityKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.FacilitySourceKey);
            if(!facilityKeyResult.Success)
            {
                return facilityKeyResult.ConvertTo<CreateSalesOrderConductorParameters>();
            }

            var orderItemsResult = parameters.PickOrderItems == null ? null : parameters.PickOrderItems.ToParsedParametersList();
            if(orderItemsResult != null && !orderItemsResult.Success)
            {
                return orderItemsResult.ConvertTo<CreateSalesOrderConductorParameters>();
            }

            return new SuccessResult().ConvertTo(new CreateSalesOrderConductorParameters
                {
                    CreateParameters = parameters,
                    CustomerKey = customerKey == null ? null : customerKey.ToCustomerKey(),
                    BrokerKey = brokerKey == null ? null : brokerKey.ToCompanyKey(),
                    ShipFromFacilityKey = facilityKeyResult.ResultingObject.ToFacilityKey(),
                    OrderItems = orderItemsResult == null ? null : orderItemsResult.ResultingObject
                });
        }
    }
}