namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class TransitInformation
    {
        public string ShipmentMethod { get; set; }
        public string FreightType { get; set; }
        public string DriverName { get; set; }
        public string CarrierName { get; set; }
        public string TrailerLicenseNumber { get; set; }
        public string ContainerSeal { get; set; }
    }
}