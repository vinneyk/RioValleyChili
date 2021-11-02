using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CompanyExtensions
    {
        internal static IResult<CreateCompanyCommandParameters> ToParsedParameters(this ICreateCompanyParameters createCompanyParameters)
        {
            if(createCompanyParameters == null) { throw new ArgumentNullException("createCompanyParameters"); }

            CompanyKey brokerKey = null;
            if(!string.IsNullOrEmpty(createCompanyParameters.BrokerKey))
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(createCompanyParameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<CreateCompanyCommandParameters>();
                }
                brokerKey = brokerKeyResult.ResultingObject.ToCompanyKey();
            }

            if(createCompanyParameters.CompanyTypes == null || !createCompanyParameters.CompanyTypes.Any())
            {
                return new InvalidResult<CreateCompanyCommandParameters>(null, UserMessages.CompanyTypesRequired);
            }

            return new SuccessResult<CreateCompanyCommandParameters>(new CreateCompanyCommandParameters
                {
                    Parameters = createCompanyParameters,
                    BrokerKey = brokerKey
                });
        }

        internal static IResult<UpdateCompanyCommandParameters> ToParsedParameters(this IUpdateCompanyParameters updateCompanyParameters)
        {
            if(updateCompanyParameters == null) { throw new ArgumentNullException("updateCompanyParameters"); }

            var companyKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(updateCompanyParameters.CompanyKey);
            if(!companyKeyResult.Success)
            {
                return companyKeyResult.ConvertTo((UpdateCompanyCommandParameters)null);
            }
            var companyKey = new CompanyKey(companyKeyResult.ResultingObject);

            if(updateCompanyParameters.CompanyTypes == null || !updateCompanyParameters.CompanyTypes.Any())
            {
                return new InvalidResult<UpdateCompanyCommandParameters>(null, UserMessages.CompanyTypesRequired);
            }

            CompanyKey brokerKey = null;
            if(!string.IsNullOrEmpty(updateCompanyParameters.BrokerKey))
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(updateCompanyParameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<UpdateCompanyCommandParameters>();
                }
                brokerKey = brokerKeyResult.ResultingObject.ToCompanyKey();
            }

            return new SuccessResult<UpdateCompanyCommandParameters>(new UpdateCompanyCommandParameters
            {
                Parameters = updateCompanyParameters,
                CompanyKey = companyKey,
                BrokerKey = brokerKey
            });
        }
    }
}