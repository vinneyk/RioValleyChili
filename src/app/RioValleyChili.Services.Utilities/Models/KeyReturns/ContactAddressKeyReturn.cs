using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ContactAddressKeyReturn : IContactAddressKey
    {
        internal string ContactAddressKey { get { return new ContactAddressKey(this).KeyValue; } }

        public int CompanyKey_Id { get; internal set; }

        public int ContactKey_Id { get; internal set; }

        public int ContactAddressKey_Id { get; internal set; }
    }
}