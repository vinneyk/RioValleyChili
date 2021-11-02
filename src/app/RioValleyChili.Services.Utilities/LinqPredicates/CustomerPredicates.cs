using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class CustomerPredicates
    {
        internal static Expression<Func<Customer, bool>> ByBroker(IKey<Company> brokerKey)
        {
            var brokerKeyPredicate = brokerKey.FindByPredicate;

            return c => brokerKeyPredicate.Invoke(c.Broker);
        }
    }
}