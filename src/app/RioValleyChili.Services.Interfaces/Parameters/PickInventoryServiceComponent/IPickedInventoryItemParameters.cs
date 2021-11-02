namespace RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent
{
    public interface IPickedInventoryItemParameters
    {
        string OrderItemKey { get; }
        string InventoryKey { get; }
        int Quantity { get; }
        string CustomerLotCode { get; }
        string CustomerProductCode { get; }
    }
}