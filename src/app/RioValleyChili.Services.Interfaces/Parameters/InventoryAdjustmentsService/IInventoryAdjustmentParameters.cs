namespace RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService
{
    public interface IInventoryAdjustmentParameters
    {
        string LotKey { get; }
        string WarehouseLocationKey { get; }
        string PackagingProductKey { get; }
        string TreatmentKey { get; }
        string ToteKey { get; }
        int Adjustment { get; }
    }
}