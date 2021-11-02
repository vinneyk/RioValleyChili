using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class IntraWarehouseOrderSummary
    {
        public string OrderKey { get; set; }

        public string TrackingSheetNumber { get; set; }

        public string OperatorName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}