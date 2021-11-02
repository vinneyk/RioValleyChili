using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class ShipmentDetails
    {
        public string ShipmentKey { get; set; }
        public ShipmentStatus Status { get; set; }
        public double? PalletWeight { get; set; }
        public int PalletQuantity { get; set; }
        public InventoryOrderEnum InventoryOrderEnum { get; set; }
        
        public TransitInformation Transit { get; set; }
        public ShippingInstructions ShippingInstructions { get; set; }
    }
}