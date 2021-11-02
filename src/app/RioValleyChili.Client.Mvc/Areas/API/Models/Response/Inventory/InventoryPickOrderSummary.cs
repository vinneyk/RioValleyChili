namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class InventoryPickOrderSummary
    {
        public string InventoryPickKey { get; set; }
        public int TotalQuantity { get; set; }
        public decimal PoundsOnOrder { get; set; }
    }
}