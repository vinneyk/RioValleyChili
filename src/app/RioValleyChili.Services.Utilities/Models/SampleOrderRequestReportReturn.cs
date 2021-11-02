using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderRequestReportReturn : ISampleOrderRequestReportReturn
    {
        public string SampleOrderKey { get { return SampleOrderKeyReturn.SampleOrderKey; } }
        public DateTime ShipByDate { get; internal set; }

        public string Broker { get; internal set; }
        public string ShipVia { get; internal set; }
        public string FOB { get; internal set; }
        public string ShipToCompanyName { get; set; }
        public string RequestedByCompanyName { get; set; }

        public ShippingLabel RequestedBy { get; set; }
        public ShippingLabel ShipTo { get; set; }

        public string SpecialInstructions { get; set; }

        public IEnumerable<ISampleOrderRequestItemReportReturn> Items { get; internal set; }

        internal SampleOrderKeyReturn SampleOrderKeyReturn { get; set; }
    }

    internal class SampleOrderRequestItemReportReturn : ISampleOrderRequestItemReportReturn
    {
        public string ProductCode { get; internal set; }
        public string ProductName { get; internal set; }
        public string SampleMatch { get; internal set; }
        public int Quantity { get; internal set; }
        public string Description { get; internal set; }
    }
}