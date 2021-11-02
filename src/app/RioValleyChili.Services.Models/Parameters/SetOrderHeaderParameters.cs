using System;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetOrderHeaderParameters : ISetOrderHeaderParameters
    {
        public string CustomerPurchaseOrderNumber { get; set; }
        public DateTime? DateOrderReceived { get; set; }
        public string PaymentTerms { get; set; }
        public string OrderRequestedBy { get; set; }
        public string OrderTakenBy { get; set; }
    }
}