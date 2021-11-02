using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Client.Reporting.Models
{
    public class SalesQuoteReportModel
    {
        public int? QuoteNumber { get; set; }
        public DateTime QuoteDate { get; set; }

        public ShippingLabel SoldTo { get; set; }
        public ShippingLabel ShipTo { get; set; }

        public string SourceFacilityName { get; set; }
        public string PaymentTerms { get; set; }
        public string Broker { get; set; }
        public string SpecialInstructions { get; set; }

        public IEnumerable<SalesQuoteItemReportModel> Items { get; set; }

        public int TotalQuantity { get { return Items.Select(i => i.Quantity).DefaultIfEmpty(0).Sum(); } }
        public double TotalWeight { get { return Items.Select(i => i.NetWeight).DefaultIfEmpty(0).Sum(); } }
    }

    public class SalesQuoteItemReportModel
    {
        public string ProductName { get; set; }
        public string CustomerCode { get; set; }
        public string PackagingName { get; set; }
        public string Treatment { get; set; }
        public int Quantity { get; set; }
        public double NetWeight { get; set; }
        public double NetPrice { get; set; }
    }
}