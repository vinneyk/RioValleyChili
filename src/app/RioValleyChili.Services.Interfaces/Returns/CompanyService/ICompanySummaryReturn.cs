using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.CompanyService
{
    public interface ICompanySummaryReturn : ICompanyHeaderReturn
    {
        bool Active { get; }
        IEnumerable<CompanyType> CompanyTypes { get; }
    }
}