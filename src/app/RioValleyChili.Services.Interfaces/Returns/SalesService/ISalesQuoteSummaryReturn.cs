using System;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ISalesQuoteSummaryReturn
    {
        string SalesQuoteKey { get; }
        int? QuoteNumber { get; }
        string CustomerName { get; }
        string BrokerName { get; }
        string SourceFacilityName { get; }
        DateTime QuoteDate { get; }
        DateTime? ShipmentDate { get; }
    }
}