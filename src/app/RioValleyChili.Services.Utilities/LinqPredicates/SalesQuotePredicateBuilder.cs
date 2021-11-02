using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class SalesQuotePredicateBuilder
    {
        internal static IResult<Expression<Func<SalesQuote, bool>>> BuildPredicate(PredicateBuilderFilters filters)
        {
            var predicate = PredicateBuilder.True<SalesQuote>();

            if(filters != null)
            {
                if(filters.CustomerKey != null)
                {
                    predicate = predicate.And(SalesQuotePredicates.ByCustomerKey(filters.CustomerKey).ExpandAll());
                }

                if(filters.BrokerKey != null)
                {
                    predicate = predicate.And(SalesQuotePredicates.ByBrokerKey(filters.BrokerKey).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<SalesQuote, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            public CustomerKey CustomerKey = null;
            public CompanyKey BrokerKey = null;
        }
    }
}