using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class CompanySummaryResponse : CompanyResponse
    {
        public bool Active { get; set; }
        public IEnumerable<CompanyType> CompanyTypes { get; set; }
    }
}