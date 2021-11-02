using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    [Issue("CompanyName update not allowed due to Access implications.", Flags = IssueFlags.TodoWhenAccessFreedom )]
    public class UpdateCompanyRequest 
    {
        //[Required]
        //public string CompanyName { get; set; }
        public string CompanyKey { get; internal set; }
        public bool Active { get; set; }
        public string BrokerCompanyKey { get; set; }
        public IEnumerable<CompanyType> CompanyTypes { get; set; }
    }
}