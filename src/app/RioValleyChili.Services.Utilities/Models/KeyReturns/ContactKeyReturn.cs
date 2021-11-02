using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ContactKeyReturn : IContactKey
    {
        internal string ContactKey { get { return new ContactKey(this).KeyValue; } }

        internal string CompanyKey { get { return new CompanyKey(this).KeyValue; } }

        public int CompanyKey_Id { get; internal set; }

        public int ContactKey_Id { get; internal set; }
    }
}