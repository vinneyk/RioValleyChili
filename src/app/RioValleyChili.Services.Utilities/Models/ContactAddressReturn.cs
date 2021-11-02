using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContactAddressReturn : IContactAddressReturn
    {
        public string ContactAddressKey { get { return ContactAddressKeyReturn.ContactAddressKey; } }
        public string AddressDescription { get; internal set; }

        public Address Address { get; internal set; }

        internal ContactAddressKeyReturn ContactAddressKeyReturn { get; set; }
    }
}