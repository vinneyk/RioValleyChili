using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class FilterContactsParametersExtensions
    {
        internal static IResult<ContactPredicateBuilder.PredicateBuilderFilters> ToParsedParameters(this FilterContactsParameters parameters)
        {
            if(parameters == null)
            {
                return new SuccessResult<ContactPredicateBuilder.PredicateBuilderFilters>(new ContactPredicateBuilder.PredicateBuilderFilters());
            }

            var companyKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.CompanyKey);
            if(!companyKeyResult.Success)
            {
                return companyKeyResult.ConvertTo<ContactPredicateBuilder.PredicateBuilderFilters>();
            }

            return new SuccessResult<ContactPredicateBuilder.PredicateBuilderFilters>(new ContactPredicateBuilder.PredicateBuilderFilters
                {
                    CompanyKey = companyKeyResult.ResultingObject.ToCompanyKey()
                });
        }
    }
}