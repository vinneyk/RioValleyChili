using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class InventoryShipmentOrderPredicates
    {
        internal static Expression<Func<InventoryShipmentOrder, bool>> ByOrderType(InventoryShipmentOrderTypeEnum orderType)
        {
            return i => i.OrderType == orderType;
        }

        internal static Expression<Func<InventoryShipmentOrder, bool>> ByOriginFacility(IKey<Facility> facilityKey)
        {
            var predicate = facilityKey.FindByPredicate;
            return i => predicate.Invoke(i.SourceFacility);
        }

        internal static Expression<Func<InventoryShipmentOrder, bool>> ByDestinationFacility(IKey<Facility> facilityKey)
        {
            var predicate = facilityKey.FindByPredicate;
            return i => predicate.Invoke(i.DestinationFacility);
        }

        internal static Expression<Func<InventoryShipmentOrder, bool>> ByShipmentDateRange(DateTime start, DateTime end)
        {
            end = end.AddDays(1);
            return i => i.ShipmentInformation.ShipmentDate >= start && i.ShipmentInformation.ShipmentDate < end;
        }
    }
}