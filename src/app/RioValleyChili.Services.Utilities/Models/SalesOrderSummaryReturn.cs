using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderSummaryReturn : InventoryShipmenOrderSummaryReturn, ISalesOrderSummaryReturn
    {
        public SalesOrderStatus SalesOrderStatus { get; internal set; }
        public string PaymentTerms { get; internal set; }
        public DateTime? DateOrderReceived { get; internal set; }
        public DateTime? InvoiceDate { get; internal set; }
        public bool CreditMemo { get; internal set; }
        public ICompanySummaryReturn Customer { get; internal set; }
        public ICompanySummaryReturn Broker { get; internal set; }
    }
}