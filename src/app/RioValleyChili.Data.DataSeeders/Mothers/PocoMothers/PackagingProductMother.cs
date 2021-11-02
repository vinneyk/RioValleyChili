using System.Globalization;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class PackagingProductMother : PocoMother<PackagingProduct>
    {
        public static PackagingProduct NoPackaging
        {
            get { return BuildPackagingProduct("No Packaging", null, 0); }
        }

        public static PackagingProduct Drum240LbPackaging
        {
            get { return BuildPackagingProduct("240 lb Drum", 12, 240); }
        }

        public static PackagingProduct Drum220LbPackaging
        {
            get { return BuildPackagingProduct("220 lb Drum", 11, 220); }
        }

        public static PackagingProduct Box50LbPackaging
        {
            get { return BuildPackagingProduct("50 lb Box", 5, 50); }
        }

        public static PackagingProduct Bag20LbPackaging
        {
            get { return BuildPackagingProduct("20 lb Bag", 4, 20); }
        }

        private static PackagingProduct BuildPackagingProduct(string name, int? productCode, double weight)
        {
            return new PackagingProduct
            {
                Product = new Product
                {
                    ProductCode = productCode == null ? "" : productCode.Value.ToString(CultureInfo.InvariantCulture),
                    IsActive = true,
                    Name = name,
                    ProductType = ProductTypeEnum.Packaging,
                },
                Weight = weight
            };
        }
    }
}
