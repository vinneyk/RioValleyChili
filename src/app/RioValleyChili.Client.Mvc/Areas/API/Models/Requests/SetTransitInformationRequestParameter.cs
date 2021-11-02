namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetTransitInformationRequestParameter 
    {
        public string FreightBillType { get; set; }
        public string ShipmentMethod { get; set; }
        public string DriverName { get; set; }
        public string CarrierName { get; set; }
        public string TrailerLicenseNumber { get; set; }
        public string ContainerSeal { get; set; }
    }
}