using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
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

        public ISetShipmentInformation ShipmentInformation { get; set; }
        public IEnumerable<ISalesQuoteItemParameters> Items { get; set; }
    }
}