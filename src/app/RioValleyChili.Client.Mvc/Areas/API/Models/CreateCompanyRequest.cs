using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateCompanyRequest
    {
        [Required]
        public string CompanyName { get; set; }
        public bool Active { get; set; }
        public string BrokerCompanyKey { get; set; }

        public IEnumerable<CompanyType> CompanyTypes { get; set; }
    }
}