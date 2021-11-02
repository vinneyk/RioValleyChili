using System;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Utilities.Models
{
    public class ShippingInstructions : IShippingInstructions
    {
        public DateTime? RequiredDeliveryDateTime { get; set; }
        public DateTime? ShipmentDate { get; set; }

        public string InternalNotes { get; set; }
        public string ExternalNotes { get; set; }
        public string SpecialInstructions { get; set; }

        public ShippingLabel ShipFromOrSoldToShippingLabel { get; set; }
        public ShippingLabel ShipToShippingLabel { get; set; }
        public ShippingLabel FreightBillToShippingLabel { get; set; }
    }
}