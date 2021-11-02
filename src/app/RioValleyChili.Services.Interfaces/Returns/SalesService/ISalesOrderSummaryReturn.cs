using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ISalesOrderSummaryReturn : IInventoryShipmentOrderSummaryReturn
    {
        SalesOrderStatus SalesOrderStatus { get; }
        string PaymentTerms { get; }
        DateTime? DateOrderReceived { get; }
        DateTime? InvoiceDate { get; }
        bool CreditMemo { get; }
        ICompanySummaryReturn Customer { get; }
        ICompanySummaryReturn Broker { get; }
    }
}