using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for CertificateOfAnalysisReport.
    /// </summary>
    public partial class CertificateOfAnalysisReport : Report, IEntityReport<InventoryShipmentOrderCertificateOfAnalysis>
    {
        public CertificateOfAnalysisReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtCustomerName.Value = this.Field(m => m.ReportDestinationName);
            MoveNum.Value = this.Field(m => m.ReportMoveNum);
            OrderKey.Value = this.Field(m => m.OrderKey);
            PONumber.Value = this.Field(m => m.PurchaseOrderNumber);
            ShipDate.Value = this.Field(m => m.ShipmentDate);
            OrderType.Value = this.Field(m => m.OrderTypeString);

            this.Table(ItemsList, m => m.GroupedItems)
                .AddSort(m => m.ProductCode)
                .With
                (
                    l => ItemsList_Product.Value = l.Field(m => m.FullProductName),
                    l => l.Table(ItemList_Table, m => m.Items)
                        .AddSort(m => m.LotKey)
                        .With
                        (
                            t => ItemList_Table_LotNumber.Value = t.Field(m => m.LotKey),
                            t => ItemList_Table_ProdDate.Value = t.Field(m => m.ProductionDate),
                            t => ItemList_Table_Treatment.Value = t.Field(m => m.TreatmentReturn.TreatmentNameShort),

                            t => ItemList_Table_Scan.Value = t.Field(m => m.Scan),
                            t => ItemList_Table_Asta.Value = t.Field(m => m.Asta),
                            t => ItemList_Table_WA.Value = t.Field(m => m.H2O),
                            t => ItemList_Table_Scoville.Value = t.Field(m => m.Scov),
                            t => ItemList_Table_Gluten.Value = t.Field(m => m.Gluten),

                            t => ItemList_Table_TPC.Value = t.Field(m => m.TPC),
                            t => ItemList_Table_Yeast.Value = t.Field(m => m.Yeast),
                            t => ItemList_Table_Coliform.Value = t.Field(m => m.ColiF),
                            t => ItemList_Table_EColi.Value = t.Field(m => m.EColi),
                            t => ItemList_Table_Sal.Value = t.Field(m => m.Sal),

                            t => ItemList_Table_AToxin.Value = t.Field(m => m.AToxin),
                            t => ItemList_Table_InsectParts.Value = t.Field(m => m.InsP),
                            t => ItemList_Table_RodentHairs.Value = t.Field(m => m.RodHrs),
                            t => ItemList_Table_Mold.Value = t.Field(m => m.Mold),
                            t => ItemList_Table_Ash.Value = t.Field(m => m.Ash),
                            t => ItemList_Table_AIA.Value = t.Field(m => m.AIA),
                            t => ItemList_Table_BI.Value = t.Field(m => m.BI),

                            t => ItemList_Table_Comments.Value = t.Field(m => m.Notes),
                            t => ItemList_Table_LoBacTextBox.Value = t.Field(m => m.LoBacString)
                        )
                );

        }
    }
}