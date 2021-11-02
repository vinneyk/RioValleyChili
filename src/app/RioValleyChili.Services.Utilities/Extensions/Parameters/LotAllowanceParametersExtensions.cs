using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class LotAllowanceParametersExtensions
    {
        internal static IResult<LotAllowanceParameters> ToParsedParameters(this ILotAllowanceParameters parameters)
        {
            var lotKey = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKey.Success)
            {
                return lotKey.ConvertTo<LotAllowanceParameters>();
            }

            var customerKey = string.IsNullOrWhiteSpace(parameters.CustomerKey) ? null : KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
            if(customerKey != null && !customerKey.Success)
            {
                return customerKey.ConvertTo<LotAllowanceParameters>();
            }

            var contractKey = string.IsNullOrWhiteSpace(parameters.ContractKey) ? null : KeyParserHelper.ParseResult<IContractKey>(parameters.ContractKey);
            if(contractKey != null && !contractKey.Success)
            {
                return contractKey.ConvertTo<LotAllowanceParameters>();
            }

            var customerOrderKey = string.IsNullOrWhiteSpace(parameters.CustomerOrderKey) ? null : KeyParserHelper.ParseResult<ISalesOrderKey>(parameters.CustomerOrderKey);
            if(customerOrderKey != null && !customerOrderKey.Success)
            {
                return customerOrderKey.ConvertTo<LotAllowanceParameters>();
            }

            return new SuccessResult<LotAllowanceParameters>(new LotAllowanceParameters
                {
                    LotKey = lotKey.ResultingObject.ToLotKey(),
                    CustomerKey = customerKey == null ? null : customerKey.ResultingObject.ToCustomerKey(),
                    ContractKey = contractKey == null ? null : contractKey.ResultingObject.ToContractKey(),
                    SalesOrderKey = customerOrderKey == null ? null : customerOrderKey.ResultingObject.ToSalesOrderKey()
                });
        }
    }
}