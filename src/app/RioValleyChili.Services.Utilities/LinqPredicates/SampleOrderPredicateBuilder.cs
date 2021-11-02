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
    internal class SampleOrderPredicateBuilder
    {
        public static IResult<Expression<Func<SampleOrder, bool>>> BuildPredicate(PredicateBuilderFilters filters)
        {
            var predicate = PredicateBuilder.True<SampleOrder>();

            if(filters != null)
            {
                if(filters.DateReceivedStart != null || filters.DateReceivedEnd != null)
                {
                    predicate = predicate.And(SampleOrderPredicates.ByDateReceived(filters.DateReceivedStart, filters.DateReceivedEnd).ExpandAll());
                }

                if(filters.DateCompletedStart != null || filters.DateCompletedEnd != null)
                {
                    predicate = predicate.And(SampleOrderPredicates.ByDateCompleted(filters.DateCompletedStart, filters.DateCompletedEnd).ExpandAll());
                }

                if(filters.Status != null)
                {
                    predicate = predicate.And(SampleOrderPredicates.ByStatus(filters.Status.Value).ExpandAll());
                }

                if(filters.RequestedCustomerKey != null)
                {
                    predicate = predicate.And(SampleOrderPredicates.ByRequestCustomer(filters.RequestedCustomerKey).ExpandAll());
                }

                if(filters.BrokerKey != null)
                {
                    predicate = predicate.And(SampleOrderPredicates.ByBroker(filters.BrokerKey).ExpandAll());
                }
            }

            return new SuccessResult().ConvertTo(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            public DateTime? DateReceivedStart { get; set; }
            public DateTime? DateReceivedEnd { get; set; }
            public DateTime? DateCompletedStart { get; set; }
            public DateTime? DateCompletedEnd { get; set; }
            public SampleOrderStatus? Status { get; set; }

            public CustomerKey RequestedCustomerKey { get; set; }
            public CompanyKey BrokerKey { get; set; }
        }
    }
}