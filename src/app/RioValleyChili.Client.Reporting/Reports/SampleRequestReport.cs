using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for SampleRequestReport.
    /// </summary>
    public partial class SampleRequestReport : Report, IEntityReport<SampleOrderRequestReportModel>
    {
        public SampleRequestReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtSampleNumber.Value = this.Field(m => m.SampleOrderKey);
            txtInvoiceDate.Value = this.Field(m => m.ShipByDate, "{0:M/d/yyyy}");

            txtBroker.Value = this.Field(m => m.Broker);
            txtShipVia.Value = this.Field(m => m.ShipVia);
            txtFOB.Value = this.Field(m => m.FOB);

            this.Bind(subRequestedBy, m => m.RequestedBy.Address).SubReport<AddressLabelReport>(s =>
            {
                s.Phone.Value = null;
            });
            this.Bind(subShipTo, m => m.ShipTo.Address).SubReport<AddressLabelReport>(s =>
            {
                s.Phone.Value = s.Field(m => m.Phone);
            });

            this.Table(tblItemDetails, m => m.Items)
                .With
                (
                    t => txtProductCode.Value = t.Field(m => m.ProductCode),
                    t => txtProductName.Value = t.Field(m => m.ProductName),
                    t => txtSampleMatch.Value = t.Field(m => m.SampleMatch),
                    t => txtQuantity.Value = t.Field(m => m.Quantity, "{0:#,##0}"),
                    t => txtDescription.Value = t.Field(m => m.Description),

                    t => txtTotalQuantity.Value = t.SumOfField(m => m.Quantity, "{0:#,##0}")
                );

            txtSpecialInstructions.Value = this.Field(m => m.SpecialInstructions);
            txtFooterSampleNumber.Value = this.Field(m => m.SampleOrderKey);
        }
    }
}