using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for ProductionRecapReport.
    /// </summary>
    /// http://localhost:4475/Reporting/Production/Recap/1-24-2012/1-24-2012
    public partial class ProductionRecapReport : Report, IEntityReport<ProductionRecapReportModel>
    {
        public ProductionRecapReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtHeaderDateRange.Value = this.Format("{0} thru {1}",
                this.PartialFormat("{0:MM/dd/yyyy}", this.PartialField(m => m.StartDate)),
                this.PartialFormat("{0:MM/dd/yyyy}", this.PartialField(m => m.EndDate)));

            this.Bind(subReportBatchWeights, m => m.BatchWeights).SubReport<ProductionRecapReport_WeightResults>(r => r.ByLine = false);
            this.Bind(subReportLineWeights, m => m.LineWeights).SubReport<ProductionRecapReport_WeightResults>(r => r.ByLine = true);
            this.Bind(subReportBatchTests, m => m.BatchTests).SubReport<ProductionRecapReport_TestResults>(r => r.ByLine = false);
            this.Bind(subReportBatchTimes, m => m.BatchTimes).SubReport<ProductionRecapReport_TimeResults>(r => r.ByLine = false);
            this.Bind(subReportLineTimes, m => m.LineTimes).SubReport<ProductionRecapReport_TimeResults>(r => r.ByLine = true);
            this.Bind(subReportLineTests, m => m.LineTests).SubReport<ProductionRecapReport_TestResults>(r => r.ByLine = true);

            this.Table(tblLineProduct, m => m.LineProductWeights.Items)
                .With
                (
                    t => t.Table(tblLineProduct_tblLine, m => m.Items)
                        .With
                        (
                            t2 => tblLineProduct_tblLine_txtLine.Value = t.Field(m => m.Parent.Name),

                            t2 => t2.Table(tblLineProduct_tblLine_tblProduct, m => m.Items)
                                .With
                                (
                                    t3 => tblLineProduct_tblLine_tblProduct_txtProductGroup.Value = t3.Field(m => m.Parent.Name),

                                    t3 => txtProductGroup_itemName.Value = t3.Field(m => m.Name),
                                    t3 => txtProductGroup_itemTarget.Value = t3.Field(m => m.Target),
                                    t3 => txtProductGroup_itemProduced.Value = t3.Field(m => m.Produced),
                                    t3 => txtProductGroup_itemVariance.Value = t3.Field(m => m.Delta),
                                    t3 => txtProductGroup_itemVariancePercent.Value = t3.Field(m => m.DeltaPercent),

                                    t3 => txtProductGroup_groupTarget.Value = t3.Field(m => m.Parent.Target),
                                    t3 => txtProductGroup_groupProduced.Value = t3.Field(m => m.Parent.Produced),
                                    t3 => txtProductGroup_groupVariance.Value = t3.Field(m => m.Parent.Delta),
                                    t3 => txtProductGroup_groupVariancePercent.Value = t3.Field(m => m.Parent.DeltaPercent)
                                ),

                            t2 => tblLineProduct_lineTotal.Value = t2.Field(m => m.Parent.Target),
                            t2 => tblLineProduct_lineProduced.Value = t2.Field(m => m.Parent.Produced),
                            t2 => tblLineProduct_lineVariance.Value = t2.Field(m => m.Parent.Delta),
                            t2 => tblLineProduct_lineVariancePercent.Value = t2.Field(m => m.Parent.DeltaPercent)
                        ),

                    t => tblLineProduct_totalTarget.Value = t.Field(m => m.Parent.Target),
                    t => tblLineProduct_totalProduced.Value = t.Field(m => m.Parent.Produced),
                    t => tblLineProduct_totalVariance.Value = t.Field(m => m.Parent.Delta),
                    t => tblLineProduct_totalVariancePercent.Value = t.Field(m => m.Parent.DeltaPercent)
                );

            this.Bind(subReportProductDetails, m => m.ProductDetails).SubReport<ProductionRecapReport_Details>(r => r.LineGroups = false);
            this.Bind(subReportLineDetails, m => m.LineDetails).SubReport<ProductionRecapReport_Details>(r => r.LineGroups = true);
        }
    }
}