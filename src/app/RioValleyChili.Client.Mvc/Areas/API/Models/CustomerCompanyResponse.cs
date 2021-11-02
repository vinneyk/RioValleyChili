using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CustomerCompanyResponse 
    {
        public CompanyResponse Broker { get; set; }
        public IEnumerable<CustomerCompanyNoteResponse> CustomerNotes { get; set; }
    }
}