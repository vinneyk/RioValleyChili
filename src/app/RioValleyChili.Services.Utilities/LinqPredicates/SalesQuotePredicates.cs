// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class SalesQuotePredicates
    {
        internal static Expression<Func<SalesQuote, bool>> ByCustomerKey(ICustomerKey customerKey)
        {
            var customerPredicate = customerKey.ToCustomerKey().FindByPredicate;
            return q => new[] { q.Customer }.Where(c => c != null).Any(c => customerPredicate.Invoke(c));
        }

        internal static Expression<Func<SalesQuote, bool>> ByBrokerKey(ICompanyKey customerKey)
        {
            var companyPredicate = customerKey.ToCompanyKey().FindByPredicate;
            return q => new[] { q.Broker }.Where(b => b != null).Any(b => companyPredicate.Invoke(b));
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup