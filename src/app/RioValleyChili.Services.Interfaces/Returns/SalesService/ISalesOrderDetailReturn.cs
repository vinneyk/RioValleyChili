using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ISalesOrderDetailReturn : IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<ISalesOrderItemReturn>, ISalesOrderItemReturn>
    {
        SalesOrderStatus SalesOrderStatus { get; }
        string PaymentTerms { get; }
        bool CreditMemo { get; }
        DateTime? InvoiceDate { get; }
        string InvoiceNotes { get; }
        float FreightCharge { get; }
        bool IsMiscellaneous { get; }

        ShippingLabel ShipFromReplace { get; }

        ICompanySummaryReturn Customer { get; }
        ICompanySummaryReturn Broker { get; }
    }
}