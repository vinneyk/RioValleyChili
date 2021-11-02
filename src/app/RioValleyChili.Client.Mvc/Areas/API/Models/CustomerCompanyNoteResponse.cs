using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CustomerCompanyNoteResponse
    {
        public string NoteKey { get; set; }
        public bool DisplayBold { get; set; }
        public string NoteType { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }

        public UserSummaryResponse CreatedByUser { get; set; }
    }
}