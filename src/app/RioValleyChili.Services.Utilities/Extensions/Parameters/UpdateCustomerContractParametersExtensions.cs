using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class UpdateCustomerContractParametersExtensions
    {
        internal static IResult<UpdateCustomerContractCommandParameters> ToParsedParameters(this IUpdateCustomerContractParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.ContractItems == null || !parameters.ContractItems.Any())
            {
                return new InvalidResult<UpdateCustomerContractCommandParameters>(null, UserMessages.ContractItemsRequired);
            }

            if(parameters.TermBegin >= parameters.TermEnd)
            {
                return new InvalidResult<UpdateCustomerContractCommandParameters>(null, string.Format(UserMessages.ContractTermMustBeginBeforeEnd, parameters.TermBegin, parameters.TermEnd));
            }

            var contractKeyResult = KeyParserHelper.ParseResult<IContractKey>(parameters.ContractKey);
            if(!contractKeyResult.Success)
            {
                return contractKeyResult.ConvertTo((UpdateCustomerContractCommandParameters) null);
            }

            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo((UpdateCustomerContractCommandParameters) null);
            }

            var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
            if(!brokerKeyResult.Success)
            {
                return brokerKeyResult.ConvertTo((UpdateCustomerContractCommandParameters) null);
            }

            FacilityKey facilityKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.DefaultPickFromWarehouseKey))
            {
                var warehouseKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.DefaultPickFromWarehouseKey);
                if(!warehouseKeyResult.Success)
                {
                    return warehouseKeyResult.ConvertTo((UpdateCustomerContractCommandParameters) null);
                }
                facilityKey = new FacilityKey(warehouseKeyResult.ResultingObject);
            }

            var contractItems = new List<SetContractItemParameters>();
            foreach(var item in parameters.ContractItems)
            {
                var parseItemResult = item.ToParsedParameters();
                if(!parseItemResult.Success)
                {
                    return parseItemResult.ConvertTo((UpdateCustomerContractCommandParameters) null);
                }
                contractItems.Add(parseItemResult.ResultingObject);
            }

            return new SuccessResult<UpdateCustomerContractCommandParameters>(new UpdateCustomerContractCommandParameters
                {
                    UpdateCustomerContractParameters = parameters,
                    ContractKey = new ContractKey(contractKeyResult.ResultingObject),
                    CustomerKey = new CustomerKey(customerKeyResult.ResultingObject),
                    BrokerKey = new CompanyKey(brokerKeyResult.ResultingObject),
                    DefaultPickFromFacilityKey = facilityKey,

                    ContractItems = contractItems
                });
        }
    }
}