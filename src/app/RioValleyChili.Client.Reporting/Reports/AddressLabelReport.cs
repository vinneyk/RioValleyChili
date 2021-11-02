using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for AddressLabelReport.
    /// </summary>
    public partial class AddressLabelReport : Report, IEntityReport<FacilityAddressLabel>
    {
        public AddressLabelReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            ContactName.Value = this.Field(m => m.AttentionLine);
            CustomerName.Value = this.Field(m => m.CompanyName);
            AddressLine1.Value = this.Field(m => m.AddressLine1);
            AddressLine2.Value = this.Field(m => m.AddressLine2);
            AddressLine3.Value = this.Field(m => m.AddressLine3);
            City.Value = this.Field(m => m.City);
            State.Value = this.Field(m => m.State);
            PostalCode.Value = this.Field(m => m.PostalCode);
            Phone.Value = this.Field(m => m.Phone);
        }
    }
}