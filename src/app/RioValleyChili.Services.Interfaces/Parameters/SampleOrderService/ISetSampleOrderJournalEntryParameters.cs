using System;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SampleOrderService
{
    public interface ISetSampleOrderJournalEntryParameters : IUserIdentifiable
    {
        string SampleOrderKey { get; }
        string JournalEntryKey { get; }
        DateTime? Date { get; }
        string Text { get; }
    }
}