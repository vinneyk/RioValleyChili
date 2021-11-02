using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class PickedInventory
    {
        public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> AttributeNamesByProductType { get; set; }
        public string PickedInventoryKey { get; set; }
        public IEnumerable<PickedInventoryItem> PickedInventoryItems { get; set; }
    }
}