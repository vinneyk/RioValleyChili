using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderInvoiceReturn : ISalesOrderInvoice
    {
        public int? MovementNumber { get; internal set; }
        public DateTime InvoiceDate { get; private set; }
        public string PONumber { get; internal set; }
        public float FreightCharge { get; internal set; }
        public string InvoiceNotes { get; internal set; }
        public bool CreditMemo { get; internal set; }

        public string Origin { get; internal set; }
        public DateTime? ShipDate { get; internal set; }
        public string Broker { get; internal set; }
        public string PaymentTerms { get; internal set; }
        public string Freight { get; internal set; }
        public string ShipVia { get; internal set; }

        public ShippingLabel SoldTo { get; internal set; }
        public ShippingLabel ShipTo { get; internal set; }

        public IEnumerable<ISalesOrderInvoicePickedItem> PickedItems { get; internal set; }
        public IEnumerable<ISalesOrderInvoiceOrderItem> OrderItems { get; internal set; }

        public void Initialize()
        {
            InvoiceDate = InvoiceDateReturn ?? DateTime.Now.Date;
        }

        internal DateTime? InvoiceDateReturn { get; set; }
    }

    internal class SalesOrderInvoicePickedItemReturn : ISalesOrderInvoicePickedItem
    {
        public string SalesOrderItemKey { get { return SalesOrderItemKeyReturn.SalesOrderItemKey; } }
        public string ProductCode { get; internal set; }
        public string ProductName { get; internal set; }
        public string ProductType { get; internal set; }

        public string CustomerProductCode { get; internal set; }
        public string PackagingName { get; internal set; }
        public string TreatmentNameShort { get; internal set; }
        
        public int QuantityShipped { get; internal set; }
        public double NetWeight { get; internal set; }

        public bool? LoBac { get; internal set; }

        internal SalesOrderItemKeyReturn SalesOrderItemKeyReturn { get; set; }
    }

    internal class SalesOrderInvoiceOrderItemReturn : ISalesOrderInvoiceOrderItem
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PackagingName { get; set; }
        public double NetWeight { get; set; }

        public string SalesOrderItemKey { get { return SalesOrderItemKeyReturn.SalesOrderItemKey; } }
        public string Contract
        {
            get
            {
                if(ContractId == null)
                {
                    return ContractKeyReturn == null ? null : ContractKeyReturn.ContractKey;
                }
                return ContractId.Value.ToString();
            }
        }

        public int QuantityOrdered { get; internal set; }
        public double PriceBase { get; internal set; }
        public double PriceFreight { get; internal set; }
        public double PriceTreatment { get; internal set; }
        public double PriceWarehouse { get; internal set; }
        public double PriceRebate { get; internal set; }

        internal SalesOrderItemKeyReturn SalesOrderItemKeyReturn { get; set; }
        internal int? ContractId { get; set; }
        internal ContractKeyReturn ContractKeyReturn { get; set; }
    }
}