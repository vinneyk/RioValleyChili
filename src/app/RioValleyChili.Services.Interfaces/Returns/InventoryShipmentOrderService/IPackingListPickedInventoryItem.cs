namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IPackingListPickedInventoryItem : IPickSheetItemReturn
    {
        string CustomerProductCode { get; }
        string CustomerLotCode { get; }
        double GrossWeight { get; }
    }
}