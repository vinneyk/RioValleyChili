using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ProductionRecapReport_TimeResults.
    /// </summary>
    public partial class ProductionRecapReport_TimeResults : Report, IEntityReport<TimeGroup>
    {
        public bool ByLine
        {
            set
            {
                lblTitle.Value = value ? "Time By Production Line" : "Time By Batch Type";
                lblLine.Visible = value;
                txtName.Style.TextAlign = value ? HorizontalAlign.Center : HorizontalAlign.Left;
            }
        }

        public ProductionRecapReport_TimeResults()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.Table(table1, m => m.Items)
                .With
                (
                    t => txtName.Value = t.Field(m => m.Name),
                    t => txtBudgetHrs.Value = t.Field(m => m.BudgetHrsDisplay),
                    t => txtActual.Value = t.Field(m => m.ActualDisplay),
                    t => txtDelta.Value = t.Field(m => m.DeltaDisplay),
                    t => txtDeltaPercent.Value = t.Field(m => m.DeltaPercentDisplay),

                    t => txtBudgetHrsTotal.Value = t.Field(m => m.Parent.BudgetHrsDisplay),
                    t => txtActualTotal.Value = t.Field(m => m.Parent.ActualDisplay),
                    t => txtDeltaTotal.Value = t.Field(m => m.Parent.DeltaDisplay),
                    t => txtDeltaPercentTotal.Value = t.Field(m => m.Parent.DeltaPercentDisplay)
                );
        }
    }
}