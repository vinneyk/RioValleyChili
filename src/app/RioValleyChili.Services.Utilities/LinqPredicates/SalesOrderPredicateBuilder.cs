using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class SalesOrderPredicateBuilder
    {
        internal static IResult<Expression<Func<SalesOrder, bool>>> BuildPredicate(ISalesUnitOfWork salesUnitOfWork, PredicateBuilderFilters filters)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }

            var predicate = PredicateBuilder.True<SalesOrder>();

            if(filters != null)
            {
                if(filters.SalesOrderStatus != null)
                {
                    predicate = predicate.And(CustomerOrderPredicates.ByCustomerOrderStatus(filters.SalesOrderStatus.Value).ExpandAll());
                }

                if(filters.OrderReceivedRangeStart != null || filters.OrderReceivedRangeEnd != null)
                {
                    predicate = predicate.And(CustomerOrderPredicates.ByDateOrderReceivedInRange(filters.OrderReceivedRangeStart, filters.OrderReceivedRangeEnd).ExpandAll());
                }

                if(filters.ScheduledShipDateRangeStart != null || filters.ScheduledShipDateRangeEnd != null)
                {
                    predicate = predicate.And(CustomerOrderPredicates.ByScheduledShipDateInRange(filters.ScheduledShipDateRangeStart, filters.ScheduledShipDateRangeEnd).ExpandAll());
                }

                if(filters.CustomerKey != null)
                {
                    predicate = predicate.And(CustomerOrderPredicates.ByCustomer(new CustomerKey(filters.CustomerKey)).ExpandAll());
                }

                if(filters.BrokerKey != null)
                {
                    predicate = predicate.And(CustomerOrderPredicates.ByBrokeyKey(new CompanyKey(filters.BrokerKey)).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<SalesOrder, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            public SalesOrderStatus? SalesOrderStatus = null;

            public DateTime? OrderReceivedRangeStart = null;

            public DateTime? OrderReceivedRangeEnd = null;

            public DateTime? ScheduledShipDateRangeStart = null;

            public DateTime? ScheduledShipDateRangeEnd = null;

            public ICustomerKey CustomerKey = null;

            public ICompanyKey BrokerKey = null;
        }
    }
}