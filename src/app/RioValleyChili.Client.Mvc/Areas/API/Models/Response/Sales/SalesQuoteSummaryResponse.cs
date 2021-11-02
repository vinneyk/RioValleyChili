using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesQuoteSummaryResponse
    {
        public string SalesQuoteKey { get; set; }
        public int? QuoteNumber { get; set; }
        public DateTime QuoteDate { get; set; }
        public DateTime? ScheduledShipDate { get; set; }
        public string CustomerName { get; set; }
        public string BrokerName { get; set; }
        public string SourceFacilityName { get; set; }
    }
}