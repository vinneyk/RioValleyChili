using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface ISetCompanyParameters : IUserIdentifiable
    {
        bool Active { get; }
        string BrokerKey { get; }
        IEnumerable<CompanyType> CompanyTypes { get; }
    }
}