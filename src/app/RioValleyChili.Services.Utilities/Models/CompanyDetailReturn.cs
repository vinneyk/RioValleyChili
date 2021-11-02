using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CompanyDetailReturn : CompanySummaryReturn, ICompanyDetailReturn
    {
        public ICustomerCompanyReturn Customer { get; internal set; }
    }
}