using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class SalesOrderAcknowledgement
    {
        public string OrderKey { get; set; }
        public int? MovementNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int TotalQuantity { get; set; }
        public double NetWeight { get; set; }
        public double TotalGrossWeight { get; set; }
        public double PalletWeight { get; set; }
        public double EstimatedShippingWeight { get; set; }
        public DateTime? DateReceived { get; set; }
        public string RequestedBy { get; set; }
        public string TakenBy { get; set; }
        public string PaymentTerms { get; set; }
        public string Broker { get; set; }
        public string OriginFacility { get; set; }

        public ShippingLabel SoldToShippingLabel { get; set; }
        public ShipmentInformation ShipmentInformation { get; set; }

        public IEnumerable<SalesOrderItem> PickOrderItems { get; set; }

        public double PalletWeight_Access { get { return ShipmentInformation.PalletWeight ?? ShipmentInformation.PalletQuantity * Constants.Reporting.DefaultOrderPalletWeight; } }
        public double EstimatedShippingWeight_Access { get { return TotalGrossWeight + PalletWeight_Access; } }
        public double TotalValue { get { return PickOrderItems.Select(i => i.TotalValue).DefaultIfEmpty(0).Sum(); } }
    }

    public class SalesOrderItem : InventoryPickOrderItemReturn
    {
        public string ContractItemKey { get; set; }
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }

        public override double TotalPrice { get { return PriceBase + PriceFreight + PriceTreatment + PriceWarehouse - PriceRebate; } set { } }
    }
}