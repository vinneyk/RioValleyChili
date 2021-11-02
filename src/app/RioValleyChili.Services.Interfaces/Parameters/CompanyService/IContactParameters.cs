using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface IContactParameters
    {
        string Name { get; }
        string PhoneNumber { get; }
        string EmailAddress { get; }

        IEnumerable<IContactAddressReturn> Addresses { get; }
    }
}