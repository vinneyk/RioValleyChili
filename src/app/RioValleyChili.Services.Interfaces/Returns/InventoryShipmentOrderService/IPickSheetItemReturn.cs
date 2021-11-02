using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IPickSheetItemReturn : ILocationDescription
    {
        string PickedInventoryItemKey { get; }
        string LotKey { get; }
        int Quantity { get; }
        bool? LoBac { get; }
        string CustomerProductCode { get; }
        double NetWeight { get; }
        IInventoryProductReturn LotProduct { get; }
        IPackagingProductReturn PackagingProduct { get; }
        IInventoryTreatmentReturn InventoryTreatment { get; }
    }
}