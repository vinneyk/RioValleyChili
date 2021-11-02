using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for CustomerOrderAcknowledgementReport.
    /// </summary>
    public partial class CustomerOrderAcknowledgementReport : Report, IEntityReport<SalesOrderAcknowledgement>
    {
        public CustomerOrderAcknowledgementReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            
            MoveNum.Value = this.Field(m => m.MovementNumber);
            ShipDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ShipmentDate, "{0:MMMM d, yyyy}");
            PO.Value = this.Field(m => m.PurchaseOrderNumber);
            
            this.Bind(MoveFromSubReport, o => o.SoldToShippingLabel.Address).SubReport<AddressLabelReport>();
            this.Bind(MoveToSubReport, o => o.ShipmentInformation.ShippingInstructions.ShipToShippingLabel.Address).SubReport<AddressLabelReport>();
            
            OriginFacility.Value = this.Field(m => m.OriginFacility);
            DateOrderReceived.Value = this.Field(m => m.DateReceived, "{0:M/d/yyyy}");
            Broker.Value = this.Field(m => m.Broker);

            PalletQty.Value = this.Field(m => m.ShipmentInformation.PalletQuantity);
            PaymentTerms.Value = this.Field(m => m.PaymentTerms);
            RequestedBy.Value = this.Field(m => m.RequestedBy);
            TakenBy.Value = this.Field(m => m.TakenBy);

            RequiredDeliveryDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.RequiredDeliveryDateTime, "{0:M/d/yyyy}");
            ShipVia.Value = this.Field(m => m.ShipmentInformation.TransitInformation.ShipmentMethod);
            Freight.Value = this.Field(m => m.ShipmentInformation.TransitInformation.FreightType);
            
            this.Table(ItemDetails, m => m.PickOrderItems)
                .AddSort(m => m.ProductName)
                .With
                (
                    t => Items_Product.Value = t.Field(m => m.ProductCode),
                    t => Items_ProductName.Value = t.Field(m => m.ProductName),
                    t => Items_CustomerProductCode.Value = t.Field(m => m.CustomerProductCode),
                    t => Items_Packaging.Value = t.Field(m => m.PackagingName),
                    t => Items_Treatment.Value = t.Field(m => m.TreatmentNameShort),
                    t => Items_Quantity.Value = t.Field(m => m.Quantity),
                    t => Items_NetWeight.Value = t.Field(m => m.TotalWeight, "{0:#,###}"),
                    t => Items_Price.Value = t.Field(m => m.TotalPrice, "{0:C3}"),
                    t => Items_Value.Value = t.Field(m => m.TotalValue, "{0:C}")
                );

            TotalQuantity.Value = this.Field(m => m.TotalQuantity, "{0:#,###}");
            TotalNetWeight.Value = this.Field(m => m.NetWeight, "{0:#,###}");
            TotalGrossWeight.Value = this.Field(m => m.TotalGrossWeight, "{0:#,###}");
            PalletWeight.Value = this.Field(m => m.PalletWeight_Access, "{0:#,###}");
            EstimatedShippingWeight.Value = this.Field(m => m.EstimatedShippingWeight_Access, "{0:#,###}");
            TotalValue.Value = this.Field(m => m.TotalValue, "{0:C}");

            SpecialInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.SpecialInstructions);
            BillOfLadingInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ExternalNotes);
        }
    }
}