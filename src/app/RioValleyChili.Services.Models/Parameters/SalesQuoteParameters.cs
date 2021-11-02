using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SalesQuoteParameters : ISalesQuoteParameters
    {
        public string UserToken { get; set; }

        public int? SalesQuoteNumber { get; set; }
        public string SourceFacilityKey { get; set; }
        public string CustomerKey { get; set; }
        public string BrokerKey { get; set; }
        public DateTime QuoteDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public string CalledBy { get; set; }
        public string TakenBy { get; set; }
        public string PaymentTerms { get; set; }

        public SetInventoryShipmentInformationParameters ShipmentInformation { get; set; }
        public IEnumerable<SalesQuoteItemParameters> Items { get; set; }

        ISetShipmentInformation ISalesQuoteParameters.ShipmentInformation { get { return ShipmentInformation; } }
        IEnumerable<ISalesQuoteItemParameters> ISalesQuoteParameters.Items { get { return Items; } }
    }

    public class SalesQuoteItemParameters : ISalesQuoteItemParameters 
    {
        public string SalesQuoteItemKey { get; set; }
        public int Quantity { get; set; }
        public string CustomerProductCode { get; set; }
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }

        public string ProductKey { get; set; }
        public string PackagingKey { get; set; }
        public string TreatmentKey { get; set; }
    }
}