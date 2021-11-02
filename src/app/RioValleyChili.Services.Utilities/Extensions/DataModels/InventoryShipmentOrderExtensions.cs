using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    internal static class InventoryShipmentOrderExtensions
    {
        internal static void SetHeaderParameters(this InventoryShipmentOrder order, ISetOrderHeaderParameters parameters)
        {
            order.PurchaseOrderNumber = parameters.CustomerPurchaseOrderNumber;
            order.DateReceived = parameters.DateOrderReceived;
            order.RequestedBy = parameters.OrderRequestedBy;
            order.TakenBy = parameters.OrderTakenBy;
        }
    }
}