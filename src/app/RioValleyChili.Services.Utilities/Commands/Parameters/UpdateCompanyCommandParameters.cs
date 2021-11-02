using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateCompanyCommandParameters : SetCompanyCommandParameters<IUpdateCompanyParameters>
    {
        internal CompanyKey CompanyKey { get; set; }
    }
}