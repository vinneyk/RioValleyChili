using System;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class ShippingInstructions
    {
        public DateTime? RequiredDeliveryDateTime { get; set; }
        public DateTime? ScheduledShipDateTime { get; set; }

        public string InternalNotes { get; set; }
        public string ExternalNotes { get; set; }
        public string SpecialInstructions { get; set; }

        public ShippingLabel ShipFromOrSoldTo { get; set; }
        public ShippingLabel ShipTo { get; set; }
        public ShippingLabel FreightBill { get; set; }
    }
}