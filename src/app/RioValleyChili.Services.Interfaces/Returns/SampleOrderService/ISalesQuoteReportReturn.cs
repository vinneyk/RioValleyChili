using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISalesQuoteReportReturn
    {
        int? QuoteNumber { get; }
        DateTime QuoteDate { get; }

        ShippingLabel SoldTo { get; }
        ShippingLabel ShipTo { get; }

        string SourceFacilityName { get; }
        string PaymentTerms { get; }
        string Broker { get; }
        string SpecialInstructions { get; }

        IEnumerable<ISalesQuoteItemReportReturn> Items { get; }
    }

    public interface ISalesQuoteItemReportReturn
    {
        string ProductName { get; }
        string CustomerCode { get; }
        string PackagingName { get; }
        string Treatment { get; }
        int Quantity { get; }
        double NetWeight { get; }
        double NetPrice { get; }
    }
}