using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetSampleSpecsParametersExtensions
    {
        internal static IResult<SetSampleSpecsCommandParameters> ToParsedParameters(this ISetSampleSpecsParameters parameters)
        {
            var itemKeyResult = KeyParserHelper.ParseResult<ISampleOrderItemKey>(parameters.SampleOrderItemKey);
            if(!itemKeyResult.Success)
            {
                return itemKeyResult.ConvertTo<SetSampleSpecsCommandParameters>();
            }

            return new SuccessResult<SetSampleSpecsCommandParameters>(new SetSampleSpecsCommandParameters
                {
                    Parameters = parameters,
                    SampleOrderItemKey = itemKeyResult.ResultingObject.ToSampleOrderItemKey()
                });
        }
    }
}