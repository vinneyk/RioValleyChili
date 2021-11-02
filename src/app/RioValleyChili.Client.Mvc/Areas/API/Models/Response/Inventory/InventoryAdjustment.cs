using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class InventoryAdjustment 
    {
        public string InventoryAdjustmentKey { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public IEnumerable<InventoryAdjustmentItem> Items { get; set; }
        public Notebook Notebook { get; set; }
        public string User { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}