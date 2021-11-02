using RioValleyChili.Services.Interfaces.Parameters.CompanyService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateCustomerNoteParameters : IUpdateCustomerNoteParameters
    {
        public string CustomerNoteKey { get; set; }
        public string UserToken { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Bold { get; set; }
    }
}