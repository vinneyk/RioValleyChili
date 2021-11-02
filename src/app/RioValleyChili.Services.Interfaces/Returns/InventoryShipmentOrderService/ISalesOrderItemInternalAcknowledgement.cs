namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface ISalesOrderItemInternalAcknowledgement
    {
        string CustomerOrderItemKey { get; }
        string ContractKey { get; }
        int? ContractId { get; }
        double TotalPrice { get; }
    }
}