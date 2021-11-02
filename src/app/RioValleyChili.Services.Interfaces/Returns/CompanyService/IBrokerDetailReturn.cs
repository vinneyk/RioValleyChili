using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.CompanyService
{
    public interface IBrokerDetailReturn : ICompanyDetailReturn
    {
        IEnumerable<ICompanySummaryReturn> CustomerSummaries { get; }
    }
}