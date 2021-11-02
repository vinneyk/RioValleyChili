using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CompanySummaryReturn : CompanyHeaderReturn, ICompanySummaryReturn
    {
        public bool Active { get; internal set; }
        public IEnumerable<CompanyType> CompanyTypes { get; internal set; }
    }
}