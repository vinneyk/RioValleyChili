using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ContactPredicateBuilder
    {
        internal static IResult<Expression<Func<Contact, bool>>> BuildPredicate(PredicateBuilderFilters filters)
        {
            var predicate = PredicateBuilder.True<Contact>();

            if(filters != null)
            {
                if(filters.CompanyKey != null)
                {
                    predicate = predicate.And(ContactPredicates.ByCompanyKey(filters.CompanyKey).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<Contact, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            internal CompanyKey CompanyKey = null;
        }
    }
}