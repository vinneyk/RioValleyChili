using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting.Drawing;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ProductionRecapReport_TestResults.
    /// </summary>
    public partial class ProductionRecapReport_TestResults : Report, IEntityReport<TestItemGroup>
    {
        public bool ByLine
        {
            set
            {
                lblTitle.Value = value ? "Pass / Fail By Line" : "By Testing Results";
                lblLine.Visible = value;
                txtName.Style.TextAlign = value ? HorizontalAlign.Center : HorizontalAlign.Left;
            }
        }

        public ProductionRecapReport_TestResults()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.Table(table5, m => m.Items)
                .With
                (
                    t => txtName.Value = t.Field(m => m.Name),
                    t => txtPassed.Value = t.Field(m => m.PassedDisplay),
                    t => txtFailed.Value = t.Field(m => m.FailedDisplay),
                    t => txtNonCntrl.Value = t.Field(m => m.NonCntrlDisplay),
                    t => txtInProc.Value = t.Field(m => m.InProcDisplay),
                    t => txtPassPercent.Value = t.Field(m => m.PassPercentDisplay),

                    t => txtPassedTotal.Value = t.Field(m => m.Parent.PassedDisplay),
                    t => txtFailedTotal.Value = t.Field(m => m.Parent.FailedDisplay),
                    t => txtNonCntrlTotal.Value = t.Field(m => m.Parent.NonCntrlDisplay),
                    t => txtInProcTotal.Value = t.Field(m => m.Parent.InProcDisplay),
                    t => txtPassPercentTotal.Value = t.Field(m => m.Parent.PassPercentDisplay)
                );
        }
    }
}