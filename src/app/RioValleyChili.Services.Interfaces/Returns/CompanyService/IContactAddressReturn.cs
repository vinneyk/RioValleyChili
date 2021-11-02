using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Returns.CompanyService
{
    public interface IContactAddressReturn
    {
        string ContactAddressKey { get; }
        string AddressDescription { get; }
        Address Address { get; }
    }
}