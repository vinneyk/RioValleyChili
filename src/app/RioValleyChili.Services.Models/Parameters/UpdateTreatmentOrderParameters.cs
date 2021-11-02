using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateTreatmentOrderParameters : IUpdateTreatmentOrderParameters
    {
        public string UserToken { get; set; }
        public string SourceFacilityKey { get; set; }
        public string DestinationFacilityKey { get; set; }
        public string TreatmentKey { get; set; }
        public string TreatmentOrderKey { get; set; }

        public SetOrderHeaderParameters HeaderParameters { get; set; }
        public SetInventoryShipmentInformationParameters SetShipmentInformation { get; set; }

        public IEnumerable<SetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; set; }
        public IEnumerable<SetPickedInventoryItemCodesParameters> PickedInventoryItemCodes { get; set; }

        ISetOrderHeaderParameters ISetOrderParameters.HeaderParameters { get { return HeaderParameters; } }
        ISetShipmentInformation ISetOrderParameters.SetShipmentInformation { get { return SetShipmentInformation; } }
        IEnumerable<ISetInventoryPickOrderItemParameters> ISetOrderParameters.InventoryPickOrderItems { get { return InventoryPickOrderItems; } }
        IEnumerable<ISetPickedInventoryItemCodesParameters> IUpdateTreatmentOrderParameters.PickedInventoryItemCodes { get { return PickedInventoryItemCodes; } }
    }
}