using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PickSheetItemReturn : IPickSheetItemReturn
    {
        public string LocationKey { get { return LocationKeyReturn.LocationKey; } }
        public string PickedInventoryItemKey { get { return PickedInventoryItemKeyReturn.PickedInventoryItemKey; } }
        public string LotKey { get { return LotKeyReturn.LotKey; } }

        public string Description { get; internal set; }
        public int Quantity { get; internal set; }
        public bool? LoBac { get; internal set; }
        public string CustomerProductCode { get; internal set; }
        public double NetWeight { get; internal set; }
        public IInventoryProductReturn LotProduct { get; internal set; }
        public IPackagingProductReturn PackagingProduct { get; internal set; }
        public IInventoryTreatmentReturn InventoryTreatment { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
        internal PickedInventoryItemKeyReturn PickedInventoryItemKeyReturn { get; set; }
        internal LocationKeyReturn LocationKeyReturn { get; set; }
    }
}