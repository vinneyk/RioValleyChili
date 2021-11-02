using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ProductionRecapReport_WeightResults.
    /// </summary>
    public partial class ProductionRecapReport_WeightResults : Report, IEntityReport<WeightItemGroup<WeightItem>>
    {
        public bool ByLine
        {
            set
            {
                lblTitle.Value = value ? "Lbs. By Production Line" : "Lbs. By Batch Type";
                lblLine.Visible = value;
                txtName.Style.TextAlign = value ? HorizontalAlign.Center : HorizontalAlign.Left;
            }
        }
        public ProductionRecapReport_WeightResults()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.Table(tblPoundsByBatchType, m => m.Items)
                .With
                (
                    t => txtName.Value = t.Field(m => m.Name),
                    t => txtTarget.Value = t.Field(m => m.Target),
                    t => txtProduced.Value = t.Field(m => m.Produced),
                    t => txtDelta.Value = t.Field(m => m.Delta),
                    t => txtDeltaPercent.Value = t.Field(m => m.DeltaPercent),

                    t => txtTargetTotal.Value = t.Field(m => m.Parent.Target),
                    t => txtProducedTotal.Value = t.Field(m => m.Parent.Produced),
                    t => txtDeltaTotal.Value = t.Field(m => m.Parent.Delta),
                    t => txtDeltaPercentTotal.Value = t.Field(m => m.Parent.DeltaPercent)
                );
        }
    }
}