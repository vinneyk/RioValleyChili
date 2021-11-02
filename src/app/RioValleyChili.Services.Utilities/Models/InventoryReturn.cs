using System.Linq;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryReturn : AttributesByTypeReturn, IInventoryReturn
    {
        public IQueryable<IInventorySummaryReturn> Inventory { get; internal set; }
    }
}