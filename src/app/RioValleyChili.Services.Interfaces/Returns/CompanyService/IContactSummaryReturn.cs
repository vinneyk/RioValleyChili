using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.CompanyService
{
    public interface IContactSummaryReturn
    {
        string ContactKey { get; }
        string CompanyKey { get; }
        string Name { get; }
        string PhoneNumber { get; }
        string EMailAddress { get; }

        IEnumerable<IContactAddressReturn> Addresses { get; }
    }
}