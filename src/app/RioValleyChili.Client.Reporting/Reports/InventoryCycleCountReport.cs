using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for InventoryCycleCountReport.
    /// </summary>
    public partial class InventoryCycleCountReport : Report, IEntityReport<InventoryCycleCountLocation>
    {
        public InventoryCycleCountReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtHeaderGroupWeight.Value = this.Field(m => m.Header.TotalGroupWeight);
            this.Table(tblHeaderLocations, m => m.Header.Locations)
                .With
                (
                    t => txtHeaderLocation.Value = t.Field(m => m.Location),
                    t => txtHeaderLocationWeight.Value = t.Field(m => m.LocationWeight)
                );

            txtFacility.Value = this.Field(m => m.FacilityName);
            txtGroupName.Value = this.Field(m => m.GroupName);
            txtHeaderTimestamp.Value = this.Field(m => m.ReportDateTime);

            txtLocation.Value = this.Field(m => m.Location);
            this.Table(tblInventory, m => m.Inventory)
                .With
                (
                    t => txtLot.Value = t.Field(m => m.LotKey),
                    t => txtLotDate.Value = t.Field(m => m.ProductionDate),
                    t => txtProduct.Value = t.Field(m => m.ProductDisplay),
                    t => txtPackaging.Value = t.Field(m => m.Packaging),
                    t => txtTrmt.Value = t.Field(m => m.Treatment),
                    t => txtQuantity.Value = t.Field(m => m.Quantity)
                );
            
            txtGrandTotal.Value = this.Field(m => m.LocationQuantity);

            txtFooterDate.Value = this.Field(m => m.ReportDateTime);
        }
    }
}