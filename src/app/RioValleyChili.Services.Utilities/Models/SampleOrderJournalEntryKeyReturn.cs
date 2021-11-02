using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderJournalEntryKeyReturn : ISampleOrderJournalEntryKey
    {
        public int SampleOrderKey_Year { get; internal set; }
        public int SampleOrderKey_Sequence { get; internal set; }
        public int SampleOrderJournalEntryKey_Sequence { get; internal set; }

        internal string SampleOrderJournalEntryKey { get { return new SampleOrderJournalEntryKey(this); } }
    }
}