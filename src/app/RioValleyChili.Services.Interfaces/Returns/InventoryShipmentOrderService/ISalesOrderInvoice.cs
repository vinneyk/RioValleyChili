using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface ISalesOrderInvoice
    {
        int? MovementNumber { get; }
        DateTime InvoiceDate { get; }
        string PONumber { get; }
        float FreightCharge { get; }
        string InvoiceNotes { get; }
        bool CreditMemo { get; }

        string Origin { get; }
        DateTime? ShipDate { get; }
        string Broker { get; }
        string PaymentTerms { get; }
        string Freight { get; }
        string ShipVia { get; }

        ShippingLabel SoldTo { get; }
        ShippingLabel ShipTo { get; }

        IEnumerable<ISalesOrderInvoicePickedItem> PickedItems { get; }
        IEnumerable<ISalesOrderInvoiceOrderItem> OrderItems { get; }
    }

    public interface ISalesOrderInvoicePickedItem
    {
        string SalesOrderItemKey { get; }
        
        string ProductCode { get; }
        string ProductName { get; }
        string ProductType { get; }

        string CustomerProductCode { get; }
        string PackagingName { get; }
        string TreatmentNameShort { get; }
        
        int QuantityShipped { get; }
        double NetWeight { get; }
        bool? LoBac { get; }
    }

    public interface ISalesOrderInvoiceOrderItem
    {
        string ProductCode { get; }
        string ProductName { get; }
        string PackagingName { get; }
        double NetWeight { get; }

        string SalesOrderItemKey { get; }
        string Contract { get; }

        int QuantityOrdered { get; }
        double PriceBase { get; }
        double PriceFreight { get; }
        double PriceTreatment { get; }
        double PriceWarehouse { get; }
        double PriceRebate { get; }
    }
}