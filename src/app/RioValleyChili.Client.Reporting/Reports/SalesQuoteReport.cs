using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for CustomerContract.
    /// </summary>
    public partial class SalesQuoteReport : Report, IEntityReport<SalesQuoteReportModel>
    {
        public SalesQuoteReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtSalesQuoteNumber.Value = this.Field(m => m.QuoteNumber);
            txtQuoteDate.Value = this.Field(m => m.QuoteDate, "{0:dddd, MMMM d, yyyy}");

            this.Bind(SoldToAddressLabel, m => m.SoldTo.Address).SubReport<AddressLabelReport>();
            this.Bind(ShipToAddressLabel, m => m.ShipTo.Address).SubReport<AddressLabelReport>();
            
            txtOrigin.Value = this.Field(m => m.SourceFacilityName);
            txtPaymentTerms.Value = this.Field(m => m.PaymentTerms);
            txtBroker.Value = this.Field(m => m.Broker);

            this.Table(ItemsTable, m => m.Items)
                .With
                (
                    t => Items_ProductName.Value = t.Field(m => m.ProductName),
                    t => Items_CustomerCode.Value = t.Field(m => m.CustomerCode),
                    t => Items_PackagingName.Value = t.Field(m => m.PackagingName),
                    t => Items_Treatment.Value = t.Field(m => m.Treatment),
                    t => Items_Quantity.Value = t.Field(m => m.Quantity),
                    t => Items_NetWeight.Value = t.Field(m => m.NetWeight, "{0:#,###}"),
                    t => Items_NetPrice.Value = t.Field(m => m.NetPrice, "{0:C3}")
                );

            txtTotalQuantity.Value = this.Field(m => m.TotalQuantity, "{0:#,###}");
            txtTotalWeight.Value = this.Field(m => m.TotalWeight, "{0:#,###}");

            SpecialInstructions.Value = this.Field(m => m.SpecialInstructions);
        }
    }
}