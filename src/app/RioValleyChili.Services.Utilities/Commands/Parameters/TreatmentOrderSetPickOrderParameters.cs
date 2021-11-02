using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class TreatmentOrderSetPickOrderParameters
    {
        public ISetInventoryPickOrderParameters Parameters { get; set; }

        public TreatmentOrderKey TreatmentOrderKey { get; set; }

        public List<InventoryPickOrderItemParameter> PickedItems { get; set; }
    }
}