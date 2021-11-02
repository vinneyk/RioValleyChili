using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackingListPickedInventoryItemReturn : PickSheetItemReturn, IPackingListPickedInventoryItem
    {
        public string CustomerProductCode { get; internal set; }
        public string CustomerLotCode { get; internal set; }
        public double GrossWeight { get; internal set; }
    }
}