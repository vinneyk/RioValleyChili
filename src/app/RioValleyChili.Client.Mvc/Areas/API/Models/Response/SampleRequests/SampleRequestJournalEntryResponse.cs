using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.SampleRequests
{
    public class SampleRequestJournalEntryResponse
    {
        public string JournalEntryKey { get; set; }
        public DateTime? Date { get; set; }
        public string Text { get; set; }
        public UserSummaryResponse CreatedByUser { get; set; }
    }
}