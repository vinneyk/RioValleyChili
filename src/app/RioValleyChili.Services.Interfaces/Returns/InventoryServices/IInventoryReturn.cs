using System.Linq;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IInventoryReturn : IAttributesByProductType
    {
        IQueryable<IInventorySummaryReturn> Inventory { get; }
    }
}