using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ProductionAdditiveInputs.
    /// </summary>    
    /// http://localhost:4475/Reporting/Production/Additives/5-13-2016/5-13-2016
    public partial class ProductionAdditiveInputsReport : Report, IEntityReport<ProductionAdditiveInputs>
    {
        public ProductionAdditiveInputsReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            
            txtProductionDates.Value = this.Format("{0} - {1}",
                this.PartialFormat("{0:M/d/yyyy}", this.PartialField(m => m.ProductionStart)),
                this.PartialFormat("{0:M/d/yyyy}", this.PartialField(m => m.ProductionEnd)));

            this.Table(listByDates, m => m.ByDates)
                .With
                (
                    t => txtProductionDate.Value = t.Field(m => m.ProductionDate, "{0:dddd, MMMM d, yyyy}"),
                    t => t.Table(tblTotals, m => m.Totals)
                        .With
                        (
                            t2 => txtAdditiveTotals.Value = t2.Field(m => m.AdditiveType),
                            t2 => txtTotalPounds.Value = t2.Field(m => m.TotalPoundsPicked)
                        ),
                    t => t.Table(listByOutputLots, m => m.Lots)
                        .With
                        (
                            t2 => txtOutputLot.Value = t2.Field(m => m.LotKey),
                            t2 => txtLotProduct.Value = t2.Field(m => m.Product),
                            t2 => t2.Table(listByAdditiveTypes, m => m.Additives)
                                .With
                                (
                                    t3 => t3.Table(tblItems, m => m.PickedItems)
                                        .With
                                        (
                                            t4 => txtAdditiveType.Value = t4.Field(m => m.AdditiveType),
                                            t4 => txtLotNumber.Value = t4.Field(m => m.LotKey),
                                            t4 => txtPoundsPicked.Value = t4.Field(m => m.TotalPoundsPicked),
                                            t4 => txtResultsEnteredBy.Value = t4.Field(m => m.UserResultEntered)
                                        )
                                )
                        )
                );
        }
    }
}