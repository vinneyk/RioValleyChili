using System;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetSampleOrderJournalEntryParameters : ISetSampleOrderJournalEntryParameters
    {
        public string UserToken { get; set; }
        public string SampleOrderKey { get; set; }
        public string JournalEntryKey { get; set; }
        public DateTime? Date { get; set; }
        public string Text { get; set; }
    }
}