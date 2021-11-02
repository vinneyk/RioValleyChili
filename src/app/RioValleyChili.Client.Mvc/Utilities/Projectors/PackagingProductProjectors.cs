using AutoMapper;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Client.Mvc.Utilities.Projectors
{
    public static class PackagingProductProjectors
    {
        public static PackagingProductResponse ToPackagingProductViewModel(IPackagingProductReturn packagingProduct)
        {
            return Mapper.Map<PackagingProductResponse>(packagingProduct);
        }
    }
}