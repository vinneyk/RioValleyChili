using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class LotInventory {
        public IEnumerable<InventoryItem> InventoryItems { get; set; }
    }
}