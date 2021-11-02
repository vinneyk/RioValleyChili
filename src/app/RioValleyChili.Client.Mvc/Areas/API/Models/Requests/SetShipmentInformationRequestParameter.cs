namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetShipmentInformationRequestParameter
    {
        public double? PalletWeight { get; set; }
        public int PalletQuantity { get; set; }

        public SetShippingInstructionsRequestParameter ShippingInstructions { get; set; }
        public SetTransitInformationRequestParameter Transit { get; set; }
    }
}