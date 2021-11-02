using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetOrderHeaderRequestParameter
    {
        public string CustomerPurchaseOrderNumber { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? DateOrderReceived { get; set; }
        public string PaymentTerms { get; set; }
        public string OrderRequestedBy { get; set; }
        public string OrderTakenBy { get; set; }
    }
}