using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for BillOfLadingReport.
    /// </summary>
    public partial class BillOfLadingReport : Report, IEntityReport<WarehouseOrderBillOfLading>
    {
        public BillOfLadingReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.Bind(ShipFromShippingLabel, m => m.ShipperFacilityAddress).SubReport<AddressLabelReport>(s => s.FacilityName.Value = s.Field(m => m.FacilityName));
            this.Bind(ShipToShippingLabel, m => m.ShipmentInformation.ShippingInstructions.ShipToShippingLabel.Address).SubReport<AddressLabelReport>(s => s.Phone.Value = null);
            DeliveryDueDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.RequiredDeliveryDateTime);
            ShipToPhone.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ShipToShippingLabel.Phone);
            LblTransferNumber.Value = this.Field(m => m.TransferNumberLabel);
            MoveNumber.Value = this.Field(m => m.ReportMoveNumber);
            PurchaseOrderNumber.Value = this.Field(m => m.PurchaseOrderNumber);
            MovementDate.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.ShipmentDate);
            CarrierName.Value = this.Field(m => m.ShipmentInformation.TransitInformation.CarrierName);
            TrailerNumber.Value = this.Field(m => m.ShipmentInformation.TransitInformation.TrailerLicenseNumber);
            SpecialInstructions.Value = this.Field(m => m.ShipmentInformation.ShippingInstructions.SpecialInstructions);
            this.Table(ItemsTable, m => m.Items)
                .AddSort(m => m.LotProduct.FullProductName)
                .AddSort(m => m.LotKey)
                .With
                (
                    t => Items_Quantity.Value = t.Field(m => m.Quantity),
                    t => Items_UnitPackSize.Value = t.Field(m => m.PackagingProduct.ProductName),
                    t => Items_Product.Value = t.Field(m => m.LotProduct.FullProductName),
                    t => Items_LotNumber.Value = t.Field(m => m.LotKey),
                    t => Items_LoBac.Value = t.Field(m => m.LoBacString),
                    t => Items_Trmt.Value = t.Field(m => m.InventoryTreatment.TreatmentNameShort),
                    t => Items_CustCode.Value = t.Field(m => m.CustomerProductCode),
                    t => Items_CustLot.Value = t.Field(m => m.CustomerLotCode),
                    t => Items_NetWeight.Value = t.Field(m => m.NetWeight),
                    t => Items_GrossWeight.Value = t.Field(m => m.GrossWeight)
                );
            TotalQuantity.Value = this.Field(m => m.TotalQuantity);
            PalletWeight.Value = this.Field(m => m.PalletWeight_Access);
            PalletQuantity.Value = this.Field(m => m.ShipmentInformation.PalletQuantity);
            TotalNetWeight.Value = this.Field(m => m.TotalNetWeight);
            TotalGrossWeight.Value = this.Field(m => m.TotalGrossWeight_Access);

            Prepaid.Value = this.Field(m => m.ShipmentInformation.TransitInformation.Prepaid);
            Collect.Value = this.Field(m => m.ShipmentInformation.TransitInformation.Collect);
            ThirdParty.Value = this.Field(m => m.ShipmentInformation.TransitInformation.ThirdParty);

            this.Bind(FreightBillToShippingLabel, m => m.ShipmentInformation.ShippingInstructions.FreightBillToShippingLabel.Address).SubReport<AddressLabelReport>();
            ContainerSeal.Value = this.Field(m => m.ShipmentInformation.TransitInformation.ContainerSeal);

            Footer_MoveNum.Value = this.Field(m => m.MoveNum);
        }
    }
}