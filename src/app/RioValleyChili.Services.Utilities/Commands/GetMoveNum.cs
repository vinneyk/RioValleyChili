using System;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.Commands
{
    internal class GetMoveNum
    {
        internal GetMoveNum(IRepository<InventoryShipmentOrder> inventoryShipmentOrders)
        {
            if(inventoryShipmentOrders == null) { throw new ArgumentNullException("inventoryShipmentOrders"); }
            _inventoryShipmentOrders = inventoryShipmentOrders;
        }

        internal int Get(int year)
        {
            var startRange = year * 1000;
            var endRange = (year + 1) * 1000;

            var next = (_inventoryShipmentOrders.SourceQuery
                .Where(o => o.OrderType != InventoryShipmentOrderTypeEnum.SalesOrder && o.MoveNum != null && o.MoveNum >= startRange && o.MoveNum < endRange)
                .Select(o => o.MoveNum)
                .DefaultIfEmpty(startRange).Max() ?? startRange) + 1;
            
            return next == endRange ? Get(year + 1) : next;
        }

        private readonly IRepository<InventoryShipmentOrder> _inventoryShipmentOrders;
    }
}