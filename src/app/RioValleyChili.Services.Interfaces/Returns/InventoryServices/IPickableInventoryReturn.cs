using System.Linq;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IPickableInventoryReturn
    {
        IQueryable<IPickableInventorySummaryReturn> Items { get; }
        IPickableInventoryInitializer Initializer { get; }
    }
}