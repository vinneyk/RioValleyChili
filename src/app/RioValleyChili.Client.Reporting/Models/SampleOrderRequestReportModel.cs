using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Reporting.Models
{
    public class SampleOrderRequestReportModel
    {
        public string SampleOrderKey { get; set; }
        public DateTime ShipByDate { get; set; }

        public string Broker { get; set; }
        public string ShipVia { get; set; }
        public string FOB { get; set; }

        public ShippingLabel RequestedBy { get; set; }
        public ShippingLabel ShipTo { get; set; }

        public string SpecialInstructions { get; set; }

        public IEnumerable<SampleOrderRequestItemReportModel> Items { get; set; }
    }

    public class SampleOrderRequestItemReportModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string SampleMatch { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }
}