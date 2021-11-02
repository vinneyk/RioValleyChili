using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class CompanyKeyReturn : ICompanyKey
    {
        internal string CompanyKey { get { return new CompanyKey(this).KeyValue; } }

        public int CompanyKey_Id { get; internal set; }
    }
}