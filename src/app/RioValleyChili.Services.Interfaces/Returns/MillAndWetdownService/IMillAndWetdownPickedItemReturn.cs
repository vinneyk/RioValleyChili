using System;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService
{
    public interface IMillAndWetdownPickedItemReturn
    {
        string PickedInventoryItemKey { get; }
        string InventoryKey { get; }
        string LotKey { get; }
        DateTime LotDateCreated { get; }
        string ToteKey { get; }
        int QuantityPicked { get; }
        int TotalWeightPicked { get; }
        IInventoryProductReturn LotProduct { get; }
        IPackagingProductReturn PackagingProduct { get; }
        ILocationReturn Location { get; }
    }
}