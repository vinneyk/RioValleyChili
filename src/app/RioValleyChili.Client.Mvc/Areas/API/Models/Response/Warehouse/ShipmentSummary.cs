using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class ShipmentSummary
    {
        public string ShipmentKey { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime? ScheduledShipDate { get; set; }
    }
}