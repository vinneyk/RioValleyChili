namespace RioValleyChili.Services.Interfaces.Parameters.InventoryService
{
    public interface IReceiveInventoryItemParameters
    {
        int Quantity { get; }
        string PackagingProductKey { get; }
        string WarehouseLocationKey { get; }
        string TreatmentKey { get; }
        string ToteKey { get; }
    }
}