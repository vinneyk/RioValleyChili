using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for WareouseOrderAcknowledgementReport.
    /// </summary>
    public partial class WarehouseOrderAcknowledgementReport : Report, IEntityReport<InternalOrderAcknowledgement>
    {
        public WarehouseOrderAcknowledgementReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            
            LabelTitle.Value = this.Field(m => m.ReportTitleLabel);
            LabelMoveNumber.Value = this.Field(m => m.OrderKeyLabel);
            MoveNum.Value = this.Field(m => m.MovementNumber);
            OriginFacility.Value = this.Field(m => m.OriginFacility);

            ShipDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ShipmentDate, "{0:M/d/yyyy}");
            PO.Value = this.Field(m => m.PurchaseOrderNumber);

            LabelMoveFrom.Value = this.Field(o => o.MoveFromLabel);
            this.Bind(MoveFromSubReport, o => o.MoveFromAddress).SubReport<AddressLabelReport>();

            LabelMoveTo.Value = this.Field(o => o.MoveToLabel);
            this.Bind(MoveToSubReport, o => o.ShipmentInformation.ShippingInstructions.ShipToShippingLabel.Address).SubReport<AddressLabelReport>();

            this.Bind(PanelCustomerOrder, p => p.Visible, m => m.IsSalesOrder);
            PaymentTerms.Value = this.Field(m => m.PaymentTerms);
            Broker.Value = this.Field(m => m.Broker);
            
            PalletQty.Value = this.Field(m => m.ShipmentInformation.PalletQuantity);
            DateOrderReceived.Value = this.Field(m => m.DateReceived, "{0:M/d/yyyy}");
            RequestedBy.Value = this.Field(m => m.RequestedBy);
            TakenBy.Value = this.Field(m => m.TakenBy);
            RequiredDeliveryDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.RequiredDeliveryDateTime, "{0:M/d/yyyy}");
            RequiredDeliveryTime.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.RequiredDeliveryDateTime, "{0:hh:mm tt}");
            ShipVia.Value = this.Field(m => m.ShipmentInformation.TransitInformation.ShipmentMethod);
            Freight.Value = this.Field(m => m.ShipmentInformation.TransitInformation.FreightType);
            
            this.Table(ItemDetails, m => m.PickOrderItems)
                .AddSort(m => m.ProductName)
                .With
                (
                    t => LabelCustomer.Value = t.Field(m => m.CustomerOrContractLabel),
                    t => Items_Product.Value = t.Field(m => m.ProductCode),
                    t => Items_ProductName.Value = t.Field(m => m.ProductName),
                    t => Items_Customer.Value = t.Field(m => m.CustomerOrContract),
                    t => Items_CustomerProductCode.Value = t.Field(m => m.CustomerProductCode),
                    t => Items_Packaging.Value = t.Field(m => m.PackagingName),
                    t => Items_Treatment.Value = t.Field(m => m.TreatmentNameShort),
                    t => Items_Quantity.Value = t.Field(m => m.Quantity),
                    t => Items_NetWeight.Value = t.Field(m => m.TotalWeight, "{0:#,###}")
                );

            TotalQuantity.Value = this.Field(m => m.TotalQuantity, "{0:#,###}");
            TotalNetWeight.Value = this.Field(m => m.NetWeight, "{0:#,###}");
            TotalGrossWeight.Value = this.Field(m => m.TotalGrossWeight, "{0:#,###}");
            PalletWeight.Value = this.Field(m => m.PalletWeight_Access, "{0:#,###}");
            EstimatedShippingWeight.Value = this.Field(m => m.EstimatedShippingWeight_Access, "{0:#,###}");

            this.Bind(PanelProfileNotes, p => p.Visible, m => m.IsSalesOrder);
            this.Table(TblProfileNoteGroups, m => m.GroupedCustomerNotes)
                .With
                (
                    t => TextProfileGroup.Value = t.Field(m => m.Type),
                    t => t.Table(TblProfileNotes, m => m.Notes)
                        .With
                        (
                            t2 => t2.Bind(TextProfileNote, n => n.Style.Font.Bold, m => m.Bold),
                            t2 => TextProfileNote.Value = t2.Field(m => m.Text)
                        )
                );

            SpecialInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.SpecialInstructions);
            BillOfLadingInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ExternalNotes);

            this.Bind(CustomerNotesSubReport, m => (ICustomerNotesContainer)m).SubReport<CustomerNotesSubReport>();

            this.Bind(FooterMoveNum, t => t.Format, m => m.MoveNumFormat);
            FooterMoveNum.Value = this.Field(m => m.MovementNumber);
            FooterShipDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ShipmentDate, "{0:dddd, MMMM d, yyyy}");
        }
    }
}