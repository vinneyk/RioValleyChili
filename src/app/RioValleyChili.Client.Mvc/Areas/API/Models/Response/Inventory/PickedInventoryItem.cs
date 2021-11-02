namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class PickedInventoryItem : PickableInventoryItem
    {
        public string PickedInventoryItemKey { get; set; }
        public string OrderItemKey { get; set; }
        public int QuantityPicked { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
    }
}