using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class InventoryAdjustmentItem
    {
        public string InventoryAdjustmentItemKey { get; set; }
        public int AdjustmentQuantity { get; set; }
        public InventoryProductResponse InventoryProduct { get; set; }
        public string LotKey { get; set; }
        public WarehouseLocationResponse Location { get; set; }
        public PackagingProductResponse PackagingProduct { get; set; }
        public InventoryTreatmentResponse InventoryTreatment { get; set; }
        public string ToteKey { get; set; }
    }
}