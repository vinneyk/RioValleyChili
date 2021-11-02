using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for CustomerOrderInvoiceReport.
    /// </summary>
    public partial class MiscOrderInvoiceReport : Report, IEntityReport<SalesOrderInvoice>
    {
        public MiscOrderInvoiceReport()
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

            this.Table(ItemDetails, m => m.OrderItems)
                .AddSort(m => m.ProductName)
                .With
                (
                    t => Items_Product.Trimmed().Value = t.Field(m => m.ProductCode),
                    t => Items_ProductName.Trimmed().Value = t.Field(m => m.ProductName),
                    t => Items_Packaging.Trimmed().Value = t.Field(m => m.PackagingName),
                    t => Items_Quantity.Trimmed().Value = t.Field(m => m.QuantityOrdered, "{0:#,##0}"),
                    t => Items_Price.Trimmed().Value = t.Field(m => m.TotalPrice, "{0:C2}"),
                    t => Items_Value.Trimmed().Value = t.Field(m => m.TotalValue, "{0:C2}")
                );
            
            TotalDue.Value = this.Field(o => o.TotalValueOrdered, "{0:C2}");

            InvoiceNotes.Value = this.Field(o => o.InvoiceNotes);
            pnlInvoiceNotes.Bindings.Add(new Binding("Visible", this.Field(o => o.VisibleInvoiceNotes)));
            MoveNumBottom.Value = this.Field(m => m.MovementNumber);
        }
    }
}