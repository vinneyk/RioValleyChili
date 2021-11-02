using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ProductionSchedule.
    /// </summary>
    public partial class ProductionScheduleReport : Report, IEntityReport<ProductionScheduleReportModel>
    {
        public ProductionScheduleReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtProductionDate.Value = this.Field(m => m.ProductionDate, "{0:M/d/yyyy}");

            this.Table(tblItems, m => m.ScheduledItems)
                .With
                (
                    t => txtProductionLine.Value = t.Field(m => m.DisplayLine),
                    t => txtFlushBefore.Value = t.Field(m => m.DisplayFlushBefore),
                    t => t.Bind(txtFlushBefore, c => c.Visible, m => m.FlushBefore),

                    t => t.Table(tblDetail, m => m.This)
                        .With
                        (
                            t2 => txtPSNum.Value = t2.Field(m => m.PackScheduleKey),
                            t2 => txtPSNum.Value = t2.Field(m => m.PackScheduleKey),
                            t2 => txtProduct.Value = t2.Field(m => m.ProductName),
                            t2 => txtCustomer.Value = t2.Field(m => m.CustomerName),
                            t2 => txtBatches.Value = t2.Field(m => m.BatchCount, "{0:#,##0}"),
                            t2 => txtWorkType.Value = t2.Field(m => m.WorkType),

                            t2 => t2.Table(tblBatches, m => m.ProductionBatches)
                                .With
                                (
                                    t3 => txtLotNumber.Value = t3.Field(m => m.LotNumber)
                                ),
                            t2 => t2.Table(tblDetails, m => m.Details)
                                .With
                                (
                                    t3 => txtDetailName.Value = t3.Field(m => m.Name),
                                    t3 => txtDetailValue.Value = t3.Field(m => m.Value)
                                ),

                            t2 => txtPackaging.Value = t2.Field(m => m.PackagingProduct)
                        ),
                    
                    t => txtFlushAfter.Value = t.Field(m => m.DisplayFlushAfter),
                    t => t.Bind(txtFlushAfter, c => c.Visible, m => m.FlushAfter)
                );

            txtFooterProductionDate.Value = this.Field(m => m.ProductionDate, "{0:dddd, MM/dd/yyyy}");
        }
    }
}