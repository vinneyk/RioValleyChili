using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class ContactAddress 
    {
        public string ContactAddressKey { get; set; }
        public string AddressDescription { get; set; }
        public Address Address { get; set; }
    }
}