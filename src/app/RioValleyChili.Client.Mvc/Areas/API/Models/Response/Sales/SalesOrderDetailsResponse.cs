using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesOrderDetailsResponse : InventoryShipmentOrderBase<SalesOrderDetailsResponse, SalesOrderPickOrderDetail, SalesOrderPickOrderItemResponse>
    {
        public SalesOrderStatus SalesOrderStatus { get; set; }
        public string PaymentTerms { get; set; }
        public int? OrderNum { get; set; }
        public bool CreditMemo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNotes { get; set; }
        public float FreightCharge { get; set; }
        public bool IsMiscellaneous { get; set; }

        public CompanySummaryResponse Customer { get; set; }
        public CompanySummaryResponse Broker { get; set; }
    }
}