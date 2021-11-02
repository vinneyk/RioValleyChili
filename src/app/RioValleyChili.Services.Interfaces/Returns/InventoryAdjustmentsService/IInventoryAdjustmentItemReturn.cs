using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService
{
    public interface IInventoryAdjustmentItemReturn 
    {
        string InventoryAdjustmentItemKey { get; }

        int AdjustmentQuantity { get; }

        IInventoryProductReturn InventoryProduct { get; }

        string LotKey { get; }

        ILocationReturn Location { get; }

        IPackagingProductReturn PackagingProduct { get; }

        IInventoryTreatmentReturn InventoryTreatment { get; }

        string ToteKey { get; }
    }
}