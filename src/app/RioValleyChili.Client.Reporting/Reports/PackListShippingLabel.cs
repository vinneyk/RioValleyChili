using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for PackListShippingLabel.
    /// </summary>
    public partial class PackListShippingLabel : Report, IEntityReport<TitledShippingLabel>
    {
        public PackListShippingLabel()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            AddressName.Value = this.Field(m => m.Name);
            AddressLine1.Value = this.Field(m => m.Address.AddressLine1);
            AddressLine2.Value = this.Field(m => m.Address.AddressLine2);
            AddressLine3.Value = this.Field(m => m.Address.AddressLine3);
            City.Value = this.Field(m => m.Address.City);
            State.Value = this.Field(m => m.Address.State);
            PostalCode.Value = this.Field(m => m.Address.PostalCode);
        }
    }
}