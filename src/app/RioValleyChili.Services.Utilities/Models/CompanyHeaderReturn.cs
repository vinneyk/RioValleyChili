using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CompanyHeaderReturn : ICompanyHeaderReturn
    {
        public string CompanyKey { get { return CompanyKeyReturn.CompanyKey; } }
        public string Name { get; internal set; }

        internal CompanyKeyReturn CompanyKeyReturn { get; set; }
    }
}