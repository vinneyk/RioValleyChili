using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateSalesQuoteRequest
    {
        public string SourceFacilityKey { get; set; }
        public string CustomerKey { get; set; }
        public string BrokerKey { get; set; }
        public DateTime QuoteDate { get; set; }
        public DateTime? DateReceived { get; set; }

        public string CalledBy { get; set; }
        public string TakenBy { get; set; }
        public string PaymentTerms { get; set; }

        public SetShipmentInformationRequestParameter ShipmentInformation { get; set; }
        public IEnumerable<UpdateSalesQuoteItemRequest> Items { get; set; }
    }
}