namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IPackagingProductReturn : IProductReturn
    {
        double Weight { get; }
        double PackagingWeight { get; }
        double PalletWeight { get; }
    }
}