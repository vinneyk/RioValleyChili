using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ContractPredicates
    {
        internal static Expression<Func<Contract, bool>> ByCustomerKey(ICustomerKey customerKey)
        {
            var customerPredicate = customerKey.ToCustomerKey().FindByPredicate;
            return c => customerPredicate.Invoke(c.Customer);
        }

        internal static Expression<Func<Contract, bool>> ByBrokerKey(IKey<Company> brokerKey)
        {
            var brokerPredicate = brokerKey.FindByPredicate;

            return c => brokerPredicate.Invoke(c.Broker);
        }

        internal static Expression<Func<Contract, bool>> ByContractStatus(ContractStatus contractStatus)
        {
            return c => c.ContractStatus == contractStatus;
        }

        internal static Expression<Func<Contract, bool>> ByTermBeginInRange(DateTime? rangeStart, DateTime? rangeEnd)
        {
            var dateInRange = PredicateHelper.DateInRange(rangeStart, rangeEnd);

            return c => dateInRange.Invoke(c.TermBegin);
        }
    }
}