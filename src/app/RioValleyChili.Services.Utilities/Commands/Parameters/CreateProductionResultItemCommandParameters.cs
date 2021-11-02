using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateProductionResultItemCommandParameters : IProductionResultItemParameters
    {
        public int Quantity { get; set; }
        public PackagingProductKey PackagingProductKey { get; set; }
        public LocationKey LocationKey { get; set; }
        public InventoryTreatmentKey InventoryTreatmentKey { get; set; }

        IPackagingProductKey IProductionResultItemParameters.PackagingProductKey { get { return PackagingProductKey; } }
        ILocationKey IProductionResultItemParameters.LocationKey { get { return LocationKey; } }
        IInventoryTreatmentKey IProductionResultItemParameters.InventoryTreatmentKey { get { return InventoryTreatmentKey; } }
    }
}