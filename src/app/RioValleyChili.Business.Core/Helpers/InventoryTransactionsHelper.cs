using System;
using RioValleyChili.Core;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class InventoryTransactionsHelper
    {
        public static string GetDescription(InventoryTransactionType type)
        {
            switch(type)
            {
                case InventoryTransactionType.ReceiveTreatmentOrder: return "Received inventory treatment order.";
                case InventoryTransactionType.ReceiveInventory: return "Received inventory.";
                case InventoryTransactionType.ProductionResults: return "Production results.";
                case InventoryTransactionType.InventoryAdjustment: return "Adjusted inventory.";
                case InventoryTransactionType.CreatedMillAndWetdown: return "Created mill and wetdown.";
                case InventoryTransactionType.ReceivedDehydratedMaterials: return "Dehydrated materials received.";
                case InventoryTransactionType.ReceivedOtherMaterials: return "Other raw materials received.";
                case InventoryTransactionType.InternalMovement: return "Internal movement.";

                case InventoryTransactionType.PickedForBatch: return "Picked for Production Batch.";

                case InventoryTransactionType.PostedInterWarehouseOrder:
                case InventoryTransactionType.PostedTreatmentOrder:
                case InventoryTransactionType.PostedCustomerOrder:
                case InventoryTransactionType.PostedMiscellaneousOrder:
                    return "Posted order.";

                default: throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}