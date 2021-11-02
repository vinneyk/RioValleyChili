using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionResultItemReturn : IProductionResultItemReturn
    {
        public string ProductionResultItemKey { get { return LotProductionResultItemKeyReturn.ProductionResultItemKey; } }

        public IPackagingProductReturn PackagingProduct { get; internal set; }

        public ILocationReturn Location { get; internal set; }

        public IInventoryTreatmentReturn Treatment { get; internal set; }

        public int Quantity { get; internal set; }

        internal LotProductionResultItemKeyReturn LotProductionResultItemKeyReturn { get; set; }
    }
}