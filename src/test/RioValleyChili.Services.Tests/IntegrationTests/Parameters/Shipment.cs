using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class Shipment : IShipmentDetailReturn
    {
        public string ShipmentKey { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public InventoryOrderEnum InventoryOrderEnum { get; set; }
        public double? PalletWeight { get; set; }
        public int PalletQuantity { get; set; }
        public ITransitInformation TransitInformation { get; set; }
        public IShippingInstructions ShippingInstructions { get; set; }
    }
}