using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesOrderSummaryResponse
    {
        public string MovementKey { get; set; }
        public SalesOrderStatus OrderStatus { get; set; }
        public ShipmentStatus ShipmentStatus { get; set; }
        public string PaymentTerms { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? DateOrderReceived { get; set; }
        public bool CreditMemo { get; set; }
        public int? OrderNum { get; set; }
        public CompanySummaryResponse Customer { get; set; }
        public CompanySummaryResponse Broker { get; set; }
    }
}