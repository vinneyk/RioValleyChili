using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CompanyDetailResponse
    {
        public string CompanyKey { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public CustomerCompanyResponse CustomerResponse { get; set; }
        public IEnumerable<CompanyType> CompanyTypes { get; set; }
    }
}