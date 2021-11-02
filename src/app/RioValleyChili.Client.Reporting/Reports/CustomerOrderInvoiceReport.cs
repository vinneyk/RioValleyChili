using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for CustomerOrderInvoiceReport.
    /// </summary>
    public partial class CustomerOrderInvoiceReport : Report, IEntityReport<SalesOrderInvoice>
    {
        public CustomerOrderInvoiceReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            ReportTitle.Value = this.Field(m => m.ReportTitle);
            MoveNum.Value = this.Field(m => m.MovementNumber);
            InvoiceDate.Value = this.Field(m => m.InvoiceDate, "{0:M/d/yyyy}");
            
            this.Bind(SoldToSubReport, o => o.SoldTo.Address).SubReport<AddressLabelReport>();
            this.Bind(ShipToSubReport, o => o.ShipTo.Address).SubReport<AddressLabelReport>();

            Origin.Value = this.Field(m => Origin);
            ShipDate.Value = this.Field(m => m.ShipDate, "{0:M/d/yyyy}");
            Broker.Value = this.Field(m => m.Broker);
            PaymentTerms.Value = this.Field(m => m.PaymentTerms);
            Freight.Value = this.Field(m => m.Freight);
            PO.Value = this.Field(m => m.PONumber);

            this.Table(ItemDetails, m => m.CustomerInvoicePickedItems)
                .AddSort(m => m.ProductName)
                .With
                (
                    t => Items_Product.Trimmed().Value = t.Field(m => m.ProductCode),
                    t => Items_ProductName.Trimmed().Value = t.Field(m => m.ProductName),
                    t => Items_CustomerProductCode.Trimmed().Value = t.Field(m => m.CustomerProductCode),
                    t => Items_Packaging.Trimmed().Value = t.Field(m => m.PackagingName),
                    t => Items_Treatment.Trimmed().Value = t.Field(m => m.TreatmentNameShort),
                    t => Items_Ordered.Trimmed().Value = t.Field(m => m.QuantityOrdered, "{0:#,###}"),
                    t => Items_Shipped.Trimmed().Value = t.Field(m => m.QuantityShipped, "{0:#,###}"),
                    t => Items_NetWeight.Trimmed().Value = t.Field(m => m.NetWeight, "{0:#,###}"),
                    t => Items_Price.Trimmed().Value = t.Field(m => m.TotalPrice, "{0:C3}"),
                    t => Items_Value.Trimmed().Value = t.Field(m => m.TotalValue, "{0:C}")
                );
            
            TotalShipped.Value = this.Field(o => o.TotalShipped, "{0:#,###}");
            TotalWeight.Value = this.Field(o => o.TotalWeight, "{0:#,###}");

            TotalValue.Value = this.Field(o => o.TotalValue, "{0:C}");
            FreightCharge.Value = this.Field(o => o.FreightCharge, "{0:C}");
            TotalDue.Value = this.Field(o => o.TotalDue, "{0:C}");

            InvoiceNotes.Value = this.Field(o => o.InvoiceNotes);
            pnlInvoiceNotes.Bindings.Add(new Binding("Visible", this.Field(o => o.VisibleInvoiceNotes)));
            MoveNumBottom.Value = this.Field(m => m.MovementNumber);
        }
    }
}