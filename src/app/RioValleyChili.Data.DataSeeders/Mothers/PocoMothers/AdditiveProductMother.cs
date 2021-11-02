using System.Globalization;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class AdditiveProductMother : PocoMother<AdditiveProduct>
    {
        public static AdditiveProduct Cumin
        {
            get { return BuildAdditiveProduct(AdditiveTypeMother.Cumin, "Cumin", true, 9904); }
        }

        public static AdditiveProduct Dextrose
        {
            get { return BuildAdditiveProduct(AdditiveTypeMother.Dextrose, "Dextrose", true, 9906); }
        }

        public static AdditiveProduct GarlicPowder
        {
            get { return BuildAdditiveProduct(AdditiveTypeMother.Garlic, "Garlic", true, 9903); }
        }

        public static AdditiveProduct Salt
        {
            get { return BuildAdditiveProduct(AdditiveTypeMother.Salt, "Salt", true, 9922); }
        }

        public static AdditiveProduct Sipernat22
        {
            get { return BuildAdditiveProduct(AdditiveTypeMother.SiliconeDioxide, "Sipernat 22", true, 9982); }
        }

        private static AdditiveProduct BuildAdditiveProduct(AdditiveType additiveType, string name, bool isActive, int productCode)
        {
            return new AdditiveProduct
                       {
                           AdditiveType = additiveType,
                           AdditiveTypeId = additiveType.Id,
                           Product = new Product
                                         {
                                             IsActive = isActive,
                                             Name = name,
                                             ProductCode = productCode.ToString(CultureInfo.InvariantCulture),
                                             ProductType = ProductTypeEnum.Additive,
                                         },
                       };
        }
    }
}
