using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetJournalEntryRequest
    {
        public DateTime? Date { get; set; }
        public string Text { get; set; }
    }
}