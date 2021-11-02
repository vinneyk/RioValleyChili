using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetContactAddressParameters
    {
        internal IContactAddressReturn ContactAddress { get; set; }

        internal ContactAddressKey ContactAddressKey { get; set; }
    }
}