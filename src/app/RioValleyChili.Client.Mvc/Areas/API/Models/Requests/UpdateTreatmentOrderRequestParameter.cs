using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateTreatmentOrderRequestParameter
    {
        public string SourceFacilityKey { get; set; }
        public string DestinationFacilityKey { get; set; }
        public string TreatmentKey { get; set; }
        public string TreatmentOrderKey { get; set; }
        public SetOrderHeaderRequestParameter HeaderParameters { get; set; }
        public SetShipmentInformationRequestParameter SetShipmentInformation { get; set; }
        public IEnumerable<SetInventoryPickOrderItemRequest> InventoryPickOrderItems { get; set; }
        public IEnumerable<SetPickedInventoryItemCodesRequestParameter> PickedInventoryItemCodes { get; set; }
    }
}