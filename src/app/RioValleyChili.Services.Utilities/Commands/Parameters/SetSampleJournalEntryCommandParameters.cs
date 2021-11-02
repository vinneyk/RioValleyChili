using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetSampleJournalEntryCommandParameters
    {
        internal ISetSampleOrderJournalEntryParameters Parameters;

        internal SampleOrderKey SampleOrderKey;
        internal SampleOrderJournalEntryKey JournalEntryKey;
    }
}