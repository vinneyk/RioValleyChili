using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IPickedInventoryItemReturn : IInventorySummaryReturn
    {
        string PickedInventoryItemKey { get; }
        string OrderItemKey { get; }
        int QuantityPicked { get; }
        string CustomerLotCode { get; }
        string CustomerProductCode { get; }
        ILocationReturn CurrentLocation { get; }
    }
}