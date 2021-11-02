using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class CustomerOrderPredicates
    {
        internal static Expression<Func<SalesOrder, bool>> ByCustomerOrderStatus(SalesOrderStatus status)
        {
            return o => o.OrderStatus == status;
        }

        internal static Expression<Func<SalesOrder, bool>> ByDateOrderReceivedInRange(DateTime? rangeStart, DateTime? rangeEnd)
        {
            var dateInRange = PredicateHelper.DateInRange(rangeStart, rangeEnd);
            return o => dateInRange.Invoke(o.InventoryShipmentOrder.DateReceived);
        }

        internal static Expression<Func<SalesOrder, bool>> ByScheduledShipDateInRange(DateTime? rangeStart, DateTime? rangeEnd)
        {
            var dateInRange = PredicateHelper.DateInRange(rangeStart, rangeEnd);
// ReSharper disable SimplifyConditionalTernaryExpression
            return o => o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate == null ? false : dateInRange.Invoke(o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate);
// ReSharper restore SimplifyConditionalTernaryExpression
        }

        internal static Expression<Func<SalesOrder, bool>> ByCustomer(IKey<Customer> customerKey)
        {
            var customerPredicate = customerKey.FindByPredicate;
            return o => o.Customer != null && customerPredicate.Invoke(o.Customer);
        }

        internal static Expression<Func<SalesOrder, bool>> ByBrokeyKey(IKey<Company> brokerKey)
        {
            var brokerPredicate = brokerKey.FindByPredicate;
            return o => brokerPredicate.Invoke(o.Broker);
        }

        internal static Expression<Func<SalesOrder, Contract, bool>> ByContract()
        {
            return (o, c) => o.SalesOrderItems.Any(i => i.ContractYear == c.ContractYear && i.ContractSequence == c.ContractSequence);
        }
    }
}