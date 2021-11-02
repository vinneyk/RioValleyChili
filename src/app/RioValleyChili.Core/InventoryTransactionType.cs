namespace RioValleyChili.Core
{
    public enum InventoryTransactionType
    {
        ReceiveTreatmentOrder,
        ReceiveInventory,
        ProductionResults,
        InventoryAdjustment,
        CreatedMillAndWetdown,
        ReceivedDehydratedMaterials,
        InternalMovement,
        PostedInterWarehouseOrder,
        PostedTreatmentOrder,
        PostedCustomerOrder,
        PickedForBatch,
        ReceivedOtherMaterials,
        PostedMiscellaneousOrder
    }
}