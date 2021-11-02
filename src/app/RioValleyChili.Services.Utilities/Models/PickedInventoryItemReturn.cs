using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PickedInventoryItemReturn : InventoryItemReturn, IPickedInventoryItemReturn, IMillAndWetdownPickedItemReturn
    {
        public string PickedInventoryItemKey { get { return PickedInventoryItemKeyReturn.PickedInventoryItemKey; } }
        public string OrderItemKey { get { return PickOrderItemKeyReturn == null ? null : PickOrderItemKeyReturn.InventoryPickOrderItemKey; } }
        public int QuantityPicked { get; internal set; }
        public string CustomerLotCode { get; internal set; }
        public string CustomerProductCode { get; internal set; }

        public ILocationReturn CurrentLocation { get; internal set; }

        #region IMillAndWetdownPickedItemReturn

        public int TotalWeightPicked { get; internal set; }

        #endregion

        internal PickedInventoryItemKeyReturn PickedInventoryItemKeyReturn { get; set; }
        internal InventoryPickOrderItemKeyReturn PickOrderItemKeyReturn { get; set; }
    }
}