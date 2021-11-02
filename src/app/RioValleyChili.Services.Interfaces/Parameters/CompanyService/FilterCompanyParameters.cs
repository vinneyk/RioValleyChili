using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public class FilterCompanyParameters
    {
        public CompanyType? CompanyType;
        public bool? IncludeInactive;
        public string BrokerKey;
    }
}