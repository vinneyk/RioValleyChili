using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IInventorySummaryReturn : ILotSummaryReturn
    {
        string InventoryKey { get; }
        string ToteKey { get; }
        int Quantity { get; }
        IPackagingProductReturn PackagingReceived { get; }
        IPackagingProductReturn PackagingProduct { get; }
        ILocationReturn Location { get; }
        IInventoryTreatmentReturn InventoryTreatment { get; }
    }
}