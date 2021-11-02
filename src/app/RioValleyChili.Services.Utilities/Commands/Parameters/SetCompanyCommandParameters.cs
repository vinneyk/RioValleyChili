using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetCompanyCommandParameters<T>
        where T : ISetCompanyParameters
    {
        internal T Parameters { get; set; }
        internal CompanyKey BrokerKey { get; set; }
    }
}