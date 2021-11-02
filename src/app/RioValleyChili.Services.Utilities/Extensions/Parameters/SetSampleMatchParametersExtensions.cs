using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetSampleMatchParametersExtensions
    {
        internal static IResult<SetSampleMatchCommandParameters> ToParsedParameters(this ISetSampleMatchParameters parameters)
        {
            var itemKeyResult = KeyParserHelper.ParseResult<ISampleOrderItemKey>(parameters.SampleOrderItemKey);
            if(!itemKeyResult.Success)
            {
                return itemKeyResult.ConvertTo<SetSampleMatchCommandParameters>();
            }

            return new SuccessResult<SetSampleMatchCommandParameters>(new SetSampleMatchCommandParameters
                {
                    Parameters = parameters,
                    SampleOrderItemKey = itemKeyResult.ResultingObject.ToSampleOrderItemKey()
                });
        }
    }
}