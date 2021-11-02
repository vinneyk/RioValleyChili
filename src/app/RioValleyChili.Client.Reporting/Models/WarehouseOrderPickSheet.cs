using System.Collections.Generic;
using System.Windows.Forms;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class WarehouseOrderPickSheet : ICustomerNotesContainer
    {
        public InventoryShipmentOrderTypeEnum OrderType { get; set; }
        public string OrderKey { get; set; }
        public int? MovementNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public ShippingLabel ShipFromOrSoldToShippingLabel { get; set; }
        public ShipmentInformation ShipmentInformation { get; set; }
        public IEnumerable<PickSheetItemReturn> Items { get; set; }
        public IEnumerable<CustomerNotesReturn> CustomerNotes { get; set; }

        public string ReportMovementNumber { get { return MovementNumber == null ? "" : OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? MovementNumber.Value.ToString() : MovementNumber.Value.ToString("0000-000"); } }

        public string ShipFromOrSoldToLabel { get { return OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? "Sold To" : "Ship From Warehouse"; } }
    }

    public class PickSheetItemReturn
    {
        public string PickedInventoryItemKey { get; set; }
        public string LocationKey { get; set; }
        public string Description { get; set; }

        public string Street { get; private set; }
        public int Row { get; private set; }
        public string LocationDescription { get; private set; }


        public string LotKey { get; set; }
        public int Quantity { get; set; }
        public bool? LoBac { get; set; }
        public string CustomerProductCode { get; set; }
        public double NetWeight { get; set; }
        public InventoryProductReturn LotProduct { get; set; }
        public PackagingProductReturn PackagingProduct { get; set; }
        public InventoryTreatmentReturn InventoryTreatment { get; set; }

        public CheckState LoBacCheckState { get { return (LoBac ?? false) ? CheckState.Checked : CheckState.Unchecked; } }

        public void Initialize()
        {
            string street;
            int row;
            if(LocationDescriptionHelper.GetStreetRow(Description, out street, out row))
            {
                LocationDescription = string.Format("{0}{1}", street, row);
                Street = street;
                Row = row;
            }
            else
            {
                LocationDescription = Description;
                Street = Description;
                Row = 0;
            }
        }
    }
}