using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    internal static class PackagingProductExtensions
    {
        internal static PackagingProductReturn ToPackagingProductSummary(this PackagingProduct packagingProduct)
        {
            return new PackagingProductReturn
                       {
                           ProductName = packagingProduct.Product.Name,
                           PackagingProductId = packagingProduct.Id,
                           Weight = packagingProduct.Weight,
                       };
        }
    }
}
