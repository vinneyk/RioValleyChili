using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class FilterCompanyParametersExtensions
    {
        internal static IResult<CompanyPredicateBuilder.PredicateBuilderFilters> ToParsedParameters(this FilterCompanyParameters parameters)
        {
            var result = new CompanyPredicateBuilder.PredicateBuilderFilters();

            if(parameters != null)
            {
                result.CompanyType = parameters.CompanyType;
                result.IncludeInactive = parameters.IncludeInactive ?? false;
                
                if(parameters.BrokerKey != null)
                {
                    var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                    if(!brokerKeyResult.Success)
                    {
                        return brokerKeyResult.ConvertTo<CompanyPredicateBuilder.PredicateBuilderFilters>();
                    }

                    result.BrokerKey = brokerKeyResult.ResultingObject.ToCompanyKey();
                }
            }

            return new SuccessResult<CompanyPredicateBuilder.PredicateBuilderFilters>(result);
        }
    }
}