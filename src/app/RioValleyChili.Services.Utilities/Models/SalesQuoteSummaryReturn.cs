using System;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesQuoteSummaryReturn : ISalesQuoteSummaryReturn
    {
        public string SalesQuoteKey { get { return SalesQuoteKeyReturn.SalesQuoteKey; } }
        public int? QuoteNumber { get; internal set; }
        public DateTime QuoteDate { get; internal set; }
        public DateTime? ShipmentDate { get; internal set; }
        public string CustomerName { get; internal set; }
        public string BrokerName { get; internal set; }
        public string SourceFacilityName { get; internal set; }

        internal SalesQuoteKeyReturn SalesQuoteKeyReturn { get; set; }
    }
}