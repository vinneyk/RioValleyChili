using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContactSummaryReturn : IContactSummaryReturn
    {
        public string ContactKey { get { return ContactKeyReturn.ContactKey; } }
        public string CompanyKey { get { return ContactKeyReturn.CompanyKey; } }
        public string Name { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public string EMailAddress { get; internal set; }
        
        public IEnumerable<IContactAddressReturn> Addresses { get; internal set; }
        internal ContactKeyReturn ContactKeyReturn { get; set; }
    }
}