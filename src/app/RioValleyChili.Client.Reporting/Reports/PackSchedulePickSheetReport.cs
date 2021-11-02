using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for PackSchedulePickSheetReport.
    /// </summary>
    public partial class PackSchedulePickSheetReport : Report, IEntityReport<PackSchedulePickSheetReportModel>
    {
        public PackSchedulePickSheetReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            BindHeaders();
            BindBody();
            BindFooters();
        }

        private void BindHeaders()
        {
            ReportHeader.Value = this.Field(m => m.ProductName, "Pack Schedule Pick Sheet - {0}");
            PSKey.Value = this.Field(m => m.PackScheduleKey);
            PackScheduleDate.Value = this.Field(m => m.PackScheduleDate, "{0:MM/dd/yy}");
            PackScheduleNumber.Value = this.Field(m => m.PSNum, "PS Num: {0}");
            BatchType.Value = this.Field(m => m.BatchType, "Batch Type: {0}");
            PackScheduleDescription.Value = this.Field(m => m.PackScheduleDescription);
        }

        private void BindBody()
        {
            WarehouseLocation.Value = this.Field(m => m.WarehouseLocation);
            PickedLotNumber.Value = this.Field(m => m.PickedLotNumber);
            BatchLot.Value = this.Field(m => m.BatchLotNumber);
            PickedProductName.Value = this.Field(m => m.PickedProductName);
            PackagingName.Value = this.Field(m => m.PackagingName);
            LoBac.Value = this.Field(m => m.LoBac);
            Treatment.Value = this.Field(m => m.Treatment);
            QuantityPicked.Value = this.Field(m => m.QuantityPicked, "{0:#,###}");
            PoundsPicked.Value = this.Field(m => m.PoundsPicked, "{0:#,###}");
        }

        private void BindFooters()
        {
            TotalQuantityForLot.Value = this.SumOfField(m => m.QuantityPicked, "{0:#,###}");
            TotalWeightForLot.Value = this.SumOfField(m => m.PoundsPicked, "{0:#,###}");

            TotalWeightAtLocation.Value = this.SumOfField(m => m.PoundsPicked, "{0:#,###}");
        }
    }
}