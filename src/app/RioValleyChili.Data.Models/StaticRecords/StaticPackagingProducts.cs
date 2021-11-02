using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Data.Models.StaticRecords
{
    public static class StaticPackagingProducts
    {
        public static PackagingProduct NoPackaging = new PackagingProduct
            {
                Id = 0,
                Weight = 0.0,
                Product = new Product
                    {
                        Id = 0,
                        IsActive = true,
                        Name = "No Packaging",
                        ProductType = ProductTypeEnum.Packaging
                    }
            };

        public static List<PackagingProduct> PackagingProducts = new List<PackagingProduct>
            {
                NoPackaging
            };
    }
}