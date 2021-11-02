using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService
{
    public interface IMillAndWetdownResultItemReturn
    {
        string MillAndWetdownResultItemKey { get; }

        IPackagingProductReturn PackagingProduct { get; }

        ILocationReturn Location { get; }

        int QuantityProduced { get; }

        int TotalWeightProduced { get; }
    }
}