namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface IPackagingProductParameters
    {
        double Weight { get; }
        double PackagingWeight { get; }
        double PalletWeight { get; }
    }
}