using Telerik.Reporting;
using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for PickListShippingLabel.
    /// </summary>
    public partial class PickListShippingLabel : Report, IEntityReport<ShippingLabel>
    {
        public PickListShippingLabel()
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
            Phone.Value = this.Field(m => m.Phone);
        }
    }
}