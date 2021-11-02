using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    public interface IProductionResultItemParameters
    {
        int Quantity { get; }
        IPackagingProductKey PackagingProductKey { get; }
        ILocationKey LocationKey { get; }
        IInventoryTreatmentKey InventoryTreatmentKey { get; }
    }
}