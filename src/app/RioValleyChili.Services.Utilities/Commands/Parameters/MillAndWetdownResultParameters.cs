using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateMillAndWetdownResultItemCommandParameters : IProductionResultItemParameters
    {
        public int Quantity { get; set; }

        internal LocationKey LocationKey { get; set; }
        internal PackagingProductKey PackagingProductKey { get; set; }

        IPackagingProductKey IProductionResultItemParameters.PackagingProductKey { get { return PackagingProductKey; } }
        ILocationKey IProductionResultItemParameters.LocationKey { get { return LocationKey; } }
        IInventoryTreatmentKey IProductionResultItemParameters.InventoryTreatmentKey { get { return StaticInventoryTreatments.NoTreatment; } }
    }
}