using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateTreatmentOrderParameters : IUpdateTreatmentOrderParameters
    {
        public string UserToken { get; set; }
        public string TreatmentOrderKey { get; set; }
        public string TreatmentKey { get; set; }
        public string SourceFacilityKey { get; set; }
        public string DestinationFacilityKey { get; set; }
        public ISetOrderHeaderParameters HeaderParameters { get; set; }
        public ISetShipmentInformation SetShipmentInformation { get; set; }
        public IEnumerable<ISetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; set; }
        public IEnumerable<ISetPickedInventoryItemCodesParameters> PickedInventoryItemCodes { get; set; }
    }
}