using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class CustomerOrderSummary 
    {
        public string CustomerOrderKey { get; set; }
        public SalesOrderStatus OrderStatus { get; set; }
        public DateTime DateOrderReceived { get; set; }
        public CompanySummaryResponse Customer { get; set; }
        public CompanySummaryResponse Broker { get; set; }
    }
}