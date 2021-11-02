using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ISalesQuoteParameters : IUserIdentifiable
    {
        int? SalesQuoteNumber { get; }
        string SourceFacilityKey { get; }
        string CustomerKey { get; }
        string BrokerKey { get; }
        DateTime QuoteDate { get; }
        DateTime? DateReceived { get; }
        string CalledBy { get; }
        string TakenBy { get; }
        string PaymentTerms { get; }

        ISetShipmentInformation ShipmentInformation { get; }
        IEnumerable<ISalesQuoteItemParameters> Items { get; }
    }

    public interface ISalesQuoteItemParameters
    {
        string SalesQuoteItemKey { get; }
        int Quantity { get; }
        string CustomerProductCode { get; }
        double PriceBase { get; }
        double PriceFreight { get; }
        double PriceTreatment { get; }
        double PriceWarehouse { get; }
        double PriceRebate { get; }

        string ProductKey { get; }
        string PackagingKey { get; }
        string TreatmentKey { get; }
    }
}