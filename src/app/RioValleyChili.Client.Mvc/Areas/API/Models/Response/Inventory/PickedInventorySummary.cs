namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class PickedInventorySummary 
    {
        public string PickedInventoryKey { get; set; }
        public int TotalQuantityPicked { get; set; }
        public decimal PoundsPicked { get; set; }
    }
}