namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class SetCustomerNoteRequest
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Bold { get; set; }
    }
}