using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ISalesQuoteDetailReturn
    {
        string SalesQuoteKey { get;  }
        int? QuoteNumber { get; }
        DateTime QuoteDate { get; }
        DateTime? DateReceived { get; }
        string CalledBy { get; }
        string TakenBy { get; }
        string PaymentTerms { get; }

        ShippingLabel ShipFromReplace { get; }
        IShipmentDetailReturn Shipment { get; }
        IFacilitySummaryReturn SourceFacility { get; }
        ICompanySummaryReturn Customer { get; }
        ICompanySummaryReturn Broker { get; }

        IEnumerable<ISalesQuoteItemReturn> Items { get; }
    }
}