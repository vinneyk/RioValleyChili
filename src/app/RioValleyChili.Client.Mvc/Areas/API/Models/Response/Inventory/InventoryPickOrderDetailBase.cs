using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public abstract class InventoryPickOrderDetailBase<TItem>
        where TItem : InventoryPickOrderItemResponse, new()
    {
        public string InventoryPickKey { get; set; }
        public IEnumerable<TItem> PickOrderItems { get; set; }
    }
}