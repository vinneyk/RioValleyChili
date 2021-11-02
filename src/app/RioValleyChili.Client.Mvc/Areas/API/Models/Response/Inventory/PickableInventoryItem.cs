namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class PickableInventoryItem : InventoryItem
    {
        public bool ValidForPicking { get; set; }
    }
}