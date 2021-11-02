using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class CompanyPredicateBuilder
    {
        internal static IResult<Expression<Func<Company, bool>>> BuildPredicate(PredicateBuilderFilters parameters)
        {
            var predicate = CompanyPredicates.ByIncludeInactive(parameters == null ? false : parameters.IncludeInactive);

            if(parameters != null)
            {
                if(parameters.CompanyType != null)
                {
                    predicate = predicate.And(CompanyPredicates.ByCompanyType(parameters.CompanyType.Value, parameters.IncludeInactive).ExpandAll());
                }

                if(parameters.BrokerKey != null)
                {
                    predicate = predicate.And(CompanyPredicates.ByBrokerKey(parameters.BrokerKey, parameters.IncludeInactive).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<Company, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            public CompanyType? CompanyType;
            public bool IncludeInactive;
            public CompanyKey BrokerKey;
        }
    }
}