using System.Windows.Forms;
using RioValleyChili.Client.Reporting.Models;
using Telerik.Reporting;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for WarehouseOrderPackingListReport.
    /// </summary>
    public partial class PackingListBarcodeReport : Report, IEntityReport<WarehouseOrderPackingList>
    {
        public PackingListBarcodeReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            const string numberFormat = "{0:#,##0}";
            Date.Value = this.Field(m => m.ShipmentDate, "{0:M/d/yyyy}");

            LabelMoveNumber.Value = this.Field(m => m.MoveNumLabel);
            MoveNum.Value = this.Field(m => m.MoveNumValue);
            PO.Value = this.Field(m => m.PurchaseOrderNumber);
            barcodePONumber.Value = this.Field(m => m.PurchaseOrderNumber);

            this.Bind(MoveFromOrSoldTo, m => m.ReportShipFromOrSoldTo).SubReport<PackListShippingLabel>(r => r.Label.Value = r.Field(m => m.Title));
            this.Bind(MoveTo, m => m.ShipToShippingLabel).SubReport<PackListShippingLabel>(r => r.Label.Value = "Move To");

            this.Table(TableItems, m => m.Items)
                .AddSort(i => i.LotProduct.ProductName)
                .AddSort(i => i.LotKey)
                .With
                (
                    t => Item_Quantity.Value = t.Field(m => m.Quantity),
                    t => Item_UnityPackSize.Value = t.Field(m => m.PackagingProduct.ProductName),
                    t => Item_Product.Value = t.Field(m => m.LotProduct.FullProductName),
                    t => Item_LotNum.Value = t.Field(m => m.LotKey),
                    t => Item_Treatment.Value = t.Field(m => m.InventoryTreatment.TreatmentNameShort),
                    t => Item_CustCode.Value = t.Field(m => m.CustomerProductCode),
                    t => Item_CustLot.Value = t.Field(m => m.CustomerLotCode),
                    t => Item_Gross.Value = t.Field(m => m.GrossWeight, numberFormat),
                    t => Item_NetWeight.Value = t.Field(m => m.NetWeight, numberFormat),
                    t => Item_LoBac.Value = t.Field(m => m.LoBacCheckState),
                    t => Item_Check.Value = CheckState.Unchecked,

                    t => barcodeLotNumber.Value = t.Field(m => m.LotKey),
                    t => barcodeCustomerProduct.Value = t.Field(m => m.CustomerProductCode)
                );

            TotalQuantity.Value = this.Field(m => m.TotalQuantity, numberFormat);
            PalletWeight.Value = this.Field(m => m.PalletWeight_Access, numberFormat);
            TotalGrossWeight.Value = this.Field(m => m.TotalGrossWeight_Access, numberFormat);
            TotalNetWeight.Value = this.Field(m => m.TotalNetWeight, numberFormat);

            Footer_MoveNum.Value = this.Field(m => m.MovementNumber, "{0:0000-000}");
        }
    }
}