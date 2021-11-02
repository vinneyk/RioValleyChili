using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class SetJournalEntryParametersExtensions
    {
        internal static IResult<SetSampleJournalEntryCommandParameters> ToParsedParameters(this ISetSampleOrderJournalEntryParameters parameters)
        {
            var sampleOrderKeyResult = KeyParserHelper.ParseResult<ISampleOrderKey>(parameters.SampleOrderKey);
            if(!sampleOrderKeyResult.Success)
            {
                return sampleOrderKeyResult.ConvertTo<SetSampleJournalEntryCommandParameters>();
            }

            SampleOrderJournalEntryKey journalEntryKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.JournalEntryKey))
            {
                var journalEntryKeyResult = KeyParserHelper.ParseResult<ISampleOrderJournalEntryKey>(parameters.JournalEntryKey);
                if(!journalEntryKeyResult.Success)
                {
                    return journalEntryKeyResult.ConvertTo<SetSampleJournalEntryCommandParameters>();
                }

                journalEntryKey = journalEntryKeyResult.ResultingObject.ToSampleOrderJournalEntryKey();
            }

            return new SuccessResult<SetSampleJournalEntryCommandParameters>(new SetSampleJournalEntryCommandParameters
                {
                    Parameters = parameters,
                    SampleOrderKey = sampleOrderKeyResult.ResultingObject.ToSampleOrderKey(),
                    JournalEntryKey = journalEntryKey
                });
        }
    }
}