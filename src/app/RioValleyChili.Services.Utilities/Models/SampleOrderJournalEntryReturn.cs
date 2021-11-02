using System;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderJournalEntryReturn : ISampleOrderJournalEntryReturn
    {
        public string JournalEntryKey { get { return SampleOrderJournalEntryKeyReturn.SampleOrderJournalEntryKey; } }
        public DateTime? Date { get; internal set; }
        public string Text { get; internal set; }
        public IUserSummaryReturn CreatedByUser { get; set; }

        internal SampleOrderJournalEntryKeyReturn SampleOrderJournalEntryKeyReturn { get; set; }
    }
}