using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class MillAndWetdownResultItemReturn : IMillAndWetdownResultItemReturn
    {
        public string MillAndWetdownResultItemKey { get { return ItemKeyReturn.ProductionResultItemKey; } }

        public IPackagingProductReturn PackagingProduct { get; internal set; }

        public ILocationReturn Location { get; internal set; }

        public int QuantityProduced { get; internal set; }

        public int TotalWeightProduced { get; internal set; }

        #region Internal Parts

        internal LotProductionResultItemKeyReturn ItemKeyReturn { get; set; }

        #endregion
    }
}