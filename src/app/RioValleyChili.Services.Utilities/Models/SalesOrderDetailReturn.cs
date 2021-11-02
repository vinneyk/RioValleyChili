using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderDetailReturn : InventoryShipmentOrderDetailBaseReturn, ISalesOrderDetailReturn
    {
        public SalesOrderStatus SalesOrderStatus { get; internal set; }
        public string PaymentTerms { get; internal set; }
        public bool CreditMemo { get; internal set; }
        public DateTime? InvoiceDate { get; internal set; }
        public string InvoiceNotes { get; internal set; }
        public float FreightCharge { get; internal set; }
        public bool IsMiscellaneous { get; internal set; }

        public ICompanySummaryReturn Customer { get; internal set; }
        public ICompanySummaryReturn Broker { get; internal set; }
        public ShippingLabel ShipFromReplace { get; internal set; }
        public IPickOrderDetailReturn<ISalesOrderItemReturn> PickOrder { get; internal set; }
        public override InventoryOrderEnum InventoryOrderEnum { get { return InventoryOrderEnum.CustomerOrder; } }
    }
}