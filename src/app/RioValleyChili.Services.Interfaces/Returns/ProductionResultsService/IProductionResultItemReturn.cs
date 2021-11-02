using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionResultsService
{
    public interface IProductionResultItemReturn
    {
        string ProductionResultItemKey { get; }

        IPackagingProductReturn PackagingProduct { get; }

        ILocationReturn Location { get; }

        IInventoryTreatmentReturn Treatment { get; }

        int Quantity { get; }
    }
}