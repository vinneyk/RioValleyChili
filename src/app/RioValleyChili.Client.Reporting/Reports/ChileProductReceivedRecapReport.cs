using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ChileProductReceivedRecapReport.
    /// </summary>
    public partial class ChileProductReceivedRecapReport : Report, IEntityReport<ChileMaterialsReceivedRecapReportModel>
    {
        public ChileProductReceivedRecapReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtLotNumber.Value = this.Field(m => m.LotKey);
            txtDateReceived.Value = this.Field(m => m.DateReceived);
            txtLoadNumber.Value = this.Field(m => m.LoadNumber);
            txtEmployee.Value = this.Field(m => m.EmployeeName);
            txtSupplier.Value = this.Field(m => m.Supplier);
            txtProduct.Value = this.Field(m => m.Product);
            txtPurchaseOrder.Value = this.Field(m => m.PurchaseOrder);
            txtShipperNum.Value = this.Field(m => m.ShipperNumber);

            this.Table(tblItems, m => m.Items)
                .With
                (
                    t => txtItem_Tote.Value = t.Field(m => m.Tote),
                    t => txtItem_Quantity.Value = t.Field(m => m.Quantity),
                    t => txtItem_Packaging.Value = t.Field(m => m.Packaging),
                    t => txtItem_Weight.Value = t.Field(m => m.Weight),
                    t => txtItem_Variety.Value = t.Field(m => m.Variety),
                    t => txtItem_LocaleGrown.Value = t.Field(m => m.LocaleGrown),
                    t => txtItem_Location.Value = t.Field(m => m.Location),

                    t => txtTotalTotes.Value = t.SumOfField(m => m.ToteCount),
                    t => txtTotalQuantity.Value = t.SumOfField(m => m.Quantity),
                    t => txtTotalWeight.Value = t.SumOfField(m => m.Weight)
                );
        }
    }
}