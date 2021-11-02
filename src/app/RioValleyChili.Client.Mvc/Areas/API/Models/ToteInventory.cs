using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class ToteInventory
    {
        public string ToteKey { get; set; }

        public string LotKey { get; set; }

        public InventoryProductResponse Product { get; set; }

        public IEnumerable<InventoryItem> Inventory { get; set; }
    }
}