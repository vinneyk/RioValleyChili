using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetLotPackagingReceivedParametersExtensions
    {
        internal static IResult<SetLotPackagingReceivedParameters> ToParsedParameters(this ISetLotPackagingReceivedParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<SetLotPackagingReceivedParameters>();
            }

            var packagingKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.ReceivedPackagingProductKey);
            if(!packagingKeyResult.Success)
            {
                return packagingKeyResult.ConvertTo<SetLotPackagingReceivedParameters>();
            }

            return new SuccessResult<SetLotPackagingReceivedParameters>(new SetLotPackagingReceivedParameters
                {
                    LotKey = new LotKey(lotKeyResult.ResultingObject),
                    PackagingProductKey = new PackagingProductKey(packagingKeyResult.ResultingObject)
                });
        }
    }
}