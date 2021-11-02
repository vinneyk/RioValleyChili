using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService
{
    public interface IChileMaterialsReceivedItemReturn
    {
        string ItemKey { get; }
        string GrowerCode { get; }
        string ToteKey { get; }
        int Quantity { get; }
        int TotalWeight { get; }
        string Variety { get; }
        IPackagingProductReturn PackagingProduct { get; }
        ILocationReturn Location { get; }
    }
}