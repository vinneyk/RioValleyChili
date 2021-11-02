using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class ContactSummary 
    {
        public string ContactKey { get; set; }
        public string CompanyKey { get; private set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EMailAddress { get; set; }
        public IEnumerable<ContactAddress> Addresses { get; set; }
    }
}