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
    internal static class CreateCustomerContractParametersExtensions
    {
        internal static IResult<CreateCustomerContractCommandParameters> ToParsedParameters(this ICreateCustomerContractParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.ContractItems == null || !parameters.ContractItems.Any())
            {
                return new InvalidResult<CreateCustomerContractCommandParameters>(null, UserMessages.ContractItemsRequired);
            }

            if(parameters.TermBegin != null && parameters.TermEnd != null && parameters.TermBegin >= parameters.TermEnd)
            {
                return new InvalidResult<CreateCustomerContractCommandParameters>(null, string.Format(UserMessages.ContractTermMustBeginBeforeEnd, parameters.TermBegin, parameters.TermEnd));
            }

            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo((CreateCustomerContractCommandParameters) null);
            }

            FacilityKey facilityKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.DefaultPickFromFacilityKey))
            {
                var warehouseKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.DefaultPickFromFacilityKey);
                if(!warehouseKeyResult.Success)
                {
                    return warehouseKeyResult.ConvertTo((CreateCustomerContractCommandParameters) null);
                }
                facilityKey = new FacilityKey(warehouseKeyResult.ResultingObject);
            }

            var parsedItems = new List<SetContractItemParameters>();
            foreach(var item in parameters.ContractItems)
            {
                var itemResult = item.ToParsedParameters();
                if(!itemResult.Success)
                {
                    return itemResult.ConvertTo((CreateCustomerContractCommandParameters) null);
                }
                parsedItems.Add(itemResult.ResultingObject);
            }

            return new SuccessResult<CreateCustomerContractCommandParameters>(new CreateCustomerContractCommandParameters
                {
                    CreateCustomerContractParameters = parameters,
                    CustomerKey = new CustomerKey(customerKeyResult.ResultingObject),
                    DefaultPickFromFacilityKey = facilityKey,
                    ContractItems = parsedItems
                });
        }
    }
}