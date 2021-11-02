using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ContractDrawSummaryReport.
    /// </summary>
    public partial class CustomerContractDrawSummaryReport : Report, IEntityReport<CustomerContractItemDrawSummary>
    {
        public CustomerContractDrawSummaryReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            CompanyName.Value = this.Field(m => m.CompanyName);
            ContractKey.Value = this.Field(m => m.ContractKey);
            ContractTerm.Value = this.Format("{0:M/d/yyyy} - {1:M/d/yyyy}", this.PartialField(m => m.ContractTermBegin), this.PartialField(m => m.ContractTermEnd));
            ContractType.Value = this.Field(m => m.ContractType);

            ProductName.Value = this.Field(m => m.ProductName);
            CustomerProductCode.Value = this.Field(m => m.CustomerProductCode);
            PoundsContracted.Value = this.Field(m => m.TotalPoundsContracted, "{0:#,###}");
            PoundsShipped.Value = this.Field(m => m.TotalPoundsShipped, "{0:#,###}");
            PoundsPending.Value = this.Field(m => m.TotalPoundsPending, "{0:#,###}");
            PoundsRemaining.Value = this.Field(m => m.TotalPoundsRemaining, "{0:#,###}");

            ContractTotalPoundsContracted.Value = this.SumOfField(m => m.TotalPoundsContracted, "{0:#,###}");
            ContractTotalPoundsShipped.Value = this.SumOfField(m => m.TotalPoundsShipped, "{0:#,###}");
            ContractTotalPoundsPending.Value = this.SumOfField(m => m.TotalPoundsPending, "{0:#,###}");
            ContractTotalPoundsRemaining.Value = this.SumOfField(m => m.TotalPoundsRemaining, "{0:#,###}");

            CompanyTotalPoundsContracted.Value = this.SumOfField(m => m.TotalPoundsContracted, "{0:#,###}");
            CompanyTotalPoundsShipped.Value = this.SumOfField(m => m.TotalPoundsShipped, "{0:#,###}");
            CompanyTotalPoundsPending.Value = this.SumOfField(m => m.TotalPoundsPending, "{0:#,###}");
            CompanyTotalPoundsRemaining.Value = this.SumOfField(m => m.TotalPoundsRemaining, "{0:#,###}");

            ReportTotalPoundsContracted.Value = this.SumOfField(m => m.TotalPoundsContracted, "{0:#,###}");
            ReportTotalPoundsShipped.Value = this.SumOfField(m => m.TotalPoundsShipped, "{0:#,###}");
            ReportTotalPoundsPending.Value = this.SumOfField(m => m.TotalPoundsPending, "{0:#,###}");
            ReportTotalPoundsRemaining.Value = this.SumOfField(m => m.TotalPoundsRemaining, "{0:#,###}");
        }
    }
}