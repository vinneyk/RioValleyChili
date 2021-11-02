using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackagingProductReturn : ProductBaseReturn, IPackagingProductReturn
    {
        internal int PackagingProductId { get; set; }

        public double Weight { get; set; }
        public double PackagingWeight { get; set; }
        public double PalletWeight { get; set; }
        public override LotTypeEnum LotType { get { return LotTypeEnum.Packaging; } }
    }
}