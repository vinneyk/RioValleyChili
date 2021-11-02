using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ProductionRecapReport_Details.
    /// </summary>
    public partial class ProductionRecapReport_Details : Report, IEntityReport<ProductDetailsSection>
    {
        public bool LineGroups
        {
            set
            {
                tblProductDetail_tblProductGroup_lblLine.Visible = value;
                tblProductDetail_tblProductGroup_txtLineNumber.Visible = value;
                tblProductDetail_tblProductGroup_txtGroupName.Visible = !value;
                txtTotal.Value = value ? "Line Total" : "Group Total";
                txtTitle.Value = value ? "By Line Detail" : "By Product Detail";
            }
        }

        public ProductionRecapReport_Details()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.Table(tblProductDetail, m => m.Items)
                .With
                (
                    t => t.Table(tblProductDetail_tblProductGroup, m => m.Items)
                    .With
                    (
                        t2 => tblProductDetail_tblProductGroup_txtGroupName.Value = t2.Field(m => m.Parent.Name),
                        t2 => tblProductDetail_tblProductGroup_txtLineNumber.Value = t2.Field(m => m.Parent.Name),

                        t2 => tblProductDetail_tblProductGroup_txtProductName.Value = t2.Field(m => m.Name),
                        t2 => tblProductDetail_tblProductGroup_txtLot.Value = t2.Field(m => m.Lot),
                        t2 => tblProductDetail_tblProductGroup_txtTarget.Value = t2.Field(m => m.TargetWeight),
                        t2 => tblProductDetail_tblProductGroup_txtProduced.Value = t2.Field(m => m.ProducedWeight),
                        t2 => tblProductDetail_tblProductGroup_txtLine.Value = t2.Field(m => m.Line),
                        t2 => tblProductDetail_tblProductGroup_txtShift.Value = t2.Field(m => m.Shift),
                        t2 => tblProductDetail_tblProductGroup_txtPSNum.Value = t2.Field(m => m.PSNum),
                        t2 => tblProductDetail_tblProductGroup_txtBatchType.Value = t2.Field(m => m.BatchType),
                        t2 => tblProductDetail_tblProductGroup_txtMode.Value = t2.Field(m => m.Mode),
                        t2 => tblProductDetail_tblProductGroup_txtLotStat.Value = t2.Field(m => m.LotStat),
                        t2 => tblProductDetail_tblProductGroup_txtBdgtTime.Value = t2.Field(m => m.BdgtTime),
                        t2 => tblProductDetail_tblProductGroup_txtProdTime.Value = t2.Field(m => m.ProductionTime),

                        t2 => tblProductDetail_tblProductGroup_txtTargetTotal.Value = t2.Field(m => m.Parent.TargetWeight),
                        t2 => tblProductDetail_tblProductGroup_txtProducedTotal.Value = t2.Field(m => m.Parent.ProducedWeight),
                        t2 => tblProductDetail_tblProductGroup_txtBdgtTimeTotal.Value = t2.Field(m => m.Parent.BdgtTime),
                        t2 => tblProductDetail_tblProductGroup_txtProdTimeTotal.Value = t2.Field(m => m.Parent.ProductionTime),
                        t2 => tblProductDetail_tblProductGroup_txtBdgtRate.Value = t2.Field(m => m.Parent.BdgtPoundsPerHour),
                        t2 => tblProductDetail_tblProductGroup_txtProdRate.Value = t2.Field(m => m.Parent.ProdPoundsPerHour)
                    ),
                    t => tblProductDetail_txtTarget.Value = t.Field(m => m.Parent.TargetWeight),
                    t => tblProductDetail_txtProduced.Value = t.Field(m => m.Parent.ProducedWeight),
                    t => tblProductDetail_txtBdgtTime.Value = t.Field(m => m.Parent.BdgtTime),
                    t => tblProductDetail_txtProdTime.Value = t.Field(m => m.Parent.ProductionTime),
                    t => tblProductDetail_txtBdgtRate.Value = t.Field(m => m.Parent.BdgtPoundsPerHour),
                    t => tblProductDetail_txtProdRate.Value = t.Field(m => m.Parent.ProdPoundsPerHour)
                );
        }
    }
}