using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateContactParameters : IUpdateContactParameters
    {
        public string ContactKey { get; set; }
        public string UserToken { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        public IEnumerable<IContactAddressReturn> Addresses { get; set; }
    }
}