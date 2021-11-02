using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesQuoteReportReturn : ISalesQuoteReportReturn
    {
        public int? QuoteNumber { get; internal set; }
        public DateTime QuoteDate { get; internal set; }

        public ShippingLabel SoldTo { get; internal set; }
        public ShippingLabel ShipTo { get; internal set; }

        public string SourceFacilityName { get; internal set; }
        public string PaymentTerms { get; internal set; }
        public string Broker { get; internal set; }
        public string SpecialInstructions { get; internal set; }

        public IEnumerable<ISalesQuoteItemReportReturn> Items { get; internal set; }
    }

    internal class SalesQuoteItemReportReturn : ISalesQuoteItemReportReturn
    {
        public string ProductName { get { return string.IsNullOrWhiteSpace(ProductCodeSelect) ? ProductNameSelect : string.Format("{0} {1}", ProductCodeSelect, ProductNameSelect); } }
        public string CustomerCode { get; internal set; }
        public string PackagingName { get; internal set; }
        public string Treatment { get; internal set; }
        public int Quantity { get; internal set; }
        public double NetWeight { get; internal set; }
        public double NetPrice { get; internal set; }

        internal string ProductCodeSelect { get; set; }
        internal string ProductNameSelect { get; set; }
    }
}