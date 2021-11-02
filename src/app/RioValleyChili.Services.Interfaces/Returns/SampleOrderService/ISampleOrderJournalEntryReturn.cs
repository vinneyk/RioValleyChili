using System;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderJournalEntryReturn
    {
        string JournalEntryKey { get; }
        DateTime? Date { get; }
        string Text { get; }
        IUserSummaryReturn CreatedByUser { get; }
    }
}