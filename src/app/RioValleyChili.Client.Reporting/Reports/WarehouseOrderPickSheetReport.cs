using Telerik.Reporting;
using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for WarehouseOrderPickSheetReport.
    /// </summary>
    public partial class WarehouseOrderPickSheetReport : Report, IEntityReport<WarehouseOrderPickSheet>
    {
        public WarehouseOrderPickSheetReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            MoveNum.Value = this.Field(m => m.ReportMovementNumber);
            PONumber.Value = this.Field(m => m.PurchaseOrderNumber);
            ShipmentDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ShipmentDate);
            ReqDateOfDelivery.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.RequiredDeliveryDateTime);

            ShipFromWarehouseLabel.Value = this.Field(m => m.ShipFromOrSoldToLabel);
            this.Bind(ShipFromSubReport, m => m.ShipFromOrSoldToShippingLabel).SubReport<PickListShippingLabel>(i => i.Phone.Value = null);
            this.Bind(ShipToSubReport, m => m.ShipmentInformation.ShippingInstructions.ShipToShippingLabel).SubReport<PickListShippingLabel>();
            Freight.Value = this.Field(m => m.ShipmentInformation.TransitInformation.FreightType);
            
            this.Table(tblTest, m => m.Items)
                .With
                (
                    t => ItemLocation.Value = t.Field(m => m.LocationDescription),

                    t => ItemLotKey.Value = t.Field(m => m.LotKey),
                    t => ItemQuantity.Value = t.Field(m => m.Quantity),
                    t => ItemPackaging.Value = t.Field(m => m.PackagingProduct.ProductName),
                    t => ItemProduct.Value = t.Field(m => m.LotProduct.FullProductName),
                    t => ItemCustomerCode.Value = t.Field(m => m.CustomerProductCode),
                    t => ItemLoBac.Value = t.Field(m => m.LoBacCheckState),
                    t => ItemTreatment.Value = t.Field(m => m.InventoryTreatment.TreatmentNameShort),
                    t => ItemWeight.Value = t.Field(m => m.NetWeight),

                    t => ItemTotalQuantity.Value = t.SumOfField(m => m.Quantity),
                    t => ItemTotalWeight.Value = t.SumOfField(m => m.NetWeight)
                );

            SpecialInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.SpecialInstructions);
            InternalPickingInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.InternalNotes);
            BillOfLadingInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ExternalNotes);

            this.Bind(CustomerNotesSubReport, m => (ICustomerNotesContainer)m).SubReport<CustomerNotesSubReport>();
        }
    }
}