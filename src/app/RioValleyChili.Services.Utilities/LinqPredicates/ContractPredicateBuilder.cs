using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ContractPredicateBuilder
    {
        internal static IResult<Expression<Func<Contract, bool>>> BuildPredicate(PredicateBuilderFilters filters)
        {
            var predicate = PredicateBuilder.True<Contract>();

            if(filters != null)
            {
                if(filters.CustomerKey != null)
                {
                    predicate = predicate.And(ContractPredicates.ByCustomerKey(filters.CustomerKey).ExpandAll());
                }

                if(filters.BrokerKey != null)
                {
                    predicate = predicate.And(ContractPredicates.ByBrokerKey(filters.BrokerKey.ToCompanyKey()).ExpandAll());
                }

                if(filters.ContractStatus != null)
                {
                    predicate = predicate.And(ContractPredicates.ByContractStatus(filters.ContractStatus.Value).ExpandAll());
                }

                if(filters.TermBeginRangeStart != null || filters.TermBeginRangeEnd != null)
                {
                    predicate = predicate.And(ContractPredicates.ByTermBeginInRange(filters.TermBeginRangeStart, filters.TermBeginRangeEnd).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<Contract, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            internal ICustomerKey CustomerKey = null;
            internal ICompanyKey BrokerKey = null;
            internal ContractStatus? ContractStatus = null;
            internal DateTime? TermBeginRangeStart = null;
            internal DateTime? TermBeginRangeEnd = null;
        }
    }
}