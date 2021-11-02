using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryAdjustmentItemReturn : IInventoryAdjustmentItemReturn
    {
        public string InventoryAdjustmentItemKey { get { return InventoryAdjustmentItemKeyReturn.InventoryAdjustmentItemKey; } }

        public string LotKey { get { return LotKeyReturn.LotKey; } }

        public int AdjustmentQuantity { get; internal set; }

        public string ToteKey { get; internal set; }

        public ILocationReturn Location { get; internal set; }

        public IPackagingProductReturn PackagingProduct { get; internal set; }

        public IInventoryProductReturn InventoryProduct { get; internal set; }

        public IInventoryTreatmentReturn InventoryTreatment { get; internal set; }

        #region Internal Parts

        internal InventoryAdjustmentItemKeyReturn InventoryAdjustmentItemKeyReturn { get; set; }

        internal LotKeyReturn LotKeyReturn { get; set; }

        #endregion
    }
}