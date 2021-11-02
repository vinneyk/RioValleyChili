using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateCompanyParameters : IUpdateCompanyParameters
    {
        public string UserToken { get; set; }
        public string CompanyKey { get; set; }
        public bool Active { get; set; }
        public string BrokerKey { get; set; }
        public IEnumerable<CompanyType> CompanyTypes { get; set; }
    }
}