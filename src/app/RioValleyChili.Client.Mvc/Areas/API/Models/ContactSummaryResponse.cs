using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class ContactSummaryResponse
    {
        public string ContactKey { get; set; }
        public string CompanyKey { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EMailAddress { get; set; }

        public IEnumerable<ContactAddressResponse> Addresses { get; set; }
    }
}