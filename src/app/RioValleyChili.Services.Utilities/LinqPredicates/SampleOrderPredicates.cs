// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class SampleOrderPredicates
    {
        internal static Expression<Func<SampleOrder, bool>> ByKey(ISampleOrderKey key)
        {
            return o => o.Year == key.SampleOrderKey_Year && o.Sequence == key.SampleOrderKey_Sequence;
        }

        internal static Expression<Func<SampleOrder, bool>> ByDateReceived(DateTime? dateReceivedStart, DateTime? dateReceivedEnd)
        {
            var dateInRange = PredicateHelper.DateInRange(dateReceivedStart, dateReceivedEnd);
            return o => dateInRange.Invoke(o.DateReceived);
        }

        internal static Expression<Func<SampleOrder, bool>> ByDateCompleted(DateTime? dateCompletedStart, DateTime? dateCompletedEnd)
        {
            var dateInRange = PredicateHelper.DateInRange(dateCompletedStart, dateCompletedEnd);
            return o => dateInRange.Invoke(o.DateCompleted);
        }

        internal static Expression<Func<SampleOrder, bool>> ByStatus(SampleOrderStatus status)
        {
            return o => o.Status == status;
        }

        public static Expression<Func<SampleOrder, bool>> ByRequestCustomer(CustomerKey requestedCustomerKey)
        {
            var customerPredicate = requestedCustomerKey.FindByPredicate;
            return o => new[] { o.RequestCustomer }.Where(c => c != null).Any(c => customerPredicate.Invoke(c));
        }

        public static Expression<Func<SampleOrder, bool>> ByBroker(CompanyKey brokerKey)
        {
            var brokerPredicate = brokerKey.FindByPredicate;
            return o => new[] { o.Broker }.Where(c => c != null).Any(c => brokerPredicate.Invoke(c));
        }
    }
}
// ReSharper restore ConvertClosureToMethodGroup