using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateContactRequest
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyKey { get; set; }

        public IEnumerable<ContactAddressResponse> Addresses { get; set; }
    }
}