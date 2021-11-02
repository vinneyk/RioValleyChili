using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class WarehouseOrderPackingList
    {
        public InventoryShipmentOrderTypeEnum OrderType { get; set; }
        public string OrderKey { get; set; }
        public int? MovementNumber { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int TotalQuantity { get; set; }
        public int PalletQuantity { get; set; }
        public double? PalletWeight { get; set; }
        public double ItemSumPalletWeight { get; set; }
        public double TotalGrossWeight { get; set; }
        public double TotalNetWeight { get; set; }

        public ShippingLabel ShipFromOrSoldToShippingLabel { get; set; }
        public ShippingLabel ShipToShippingLabel { get; set; }

        public IEnumerable<PackingListPickedInventoryItem> Items { get; set; }

        public string MoveNumLabel { get { return OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? "Order #" : "Move Num"; } }

        public string MoveNumValue {
            get
            {
                return MovementNumber == null ? "" : OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ?
                    MovementNumber.Value.ToString() :
                    MovementNumber.Value.ToString("0000-000");
            }
        }
        public TitledShippingLabel ReportShipFromOrSoldTo
        {
            get
            {
                if(_reportShipFromOrSoldTo == null)
                {
                    _reportShipFromOrSoldTo = new TitledShippingLabel
                        {
                            Title = OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? "Sold To" : "Move From",
                            Name = ShipFromOrSoldToShippingLabel.Name,
                            Phone = ShipFromOrSoldToShippingLabel.Phone,
                            EMail = ShipFromOrSoldToShippingLabel.EMail,
                            Fax = ShipFromOrSoldToShippingLabel.Fax,
                            Address = ShipFromOrSoldToShippingLabel.Address
                        };
                }
                return _reportShipFromOrSoldTo;
            }
        }

        [Issue("Agreed with client that the BillOfLading and PackingList reports needed to display the user-supplied ShipmentInformation.PalletWeight" +
               "when defined and default to PalletQuantity * DefaultPalletWeight (60 as of time of writing). -RI 2016-09-27",
               References = new[] { "RVCADMIN-1318", "https://solutionhead.slack.com/archives/D03BRUYU4/p1475008918000133" })]
        public double PalletWeight_Access { get { return PalletWeight ?? PalletQuantity * Constants.Reporting.DefaultOrderPalletWeight; } }
        public double TotalGrossWeight_Access { get { return PalletWeight_Access + (Items.Select(i => i.GrossWeight).DefaultIfEmpty(0).Sum()); } }

        private TitledShippingLabel _reportShipFromOrSoldTo;
    }

    public class PackingListPickedInventoryItem
    {
        public string LotKey { get; set; }
        public int Quantity { get; set; }
        public bool? LoBac { get; set; }
        public double NetWeight { get; set; }
        public InventoryProductReturn LotProduct { get; set; }
        public PackagingProductReturn PackagingProduct { get; set; }
        public InventoryTreatmentReturn InventoryTreatment { get; set; }
        public string CustomerProductCode { get; set; }
        public string CustomerLotCode { get; set; }
        public double GrossWeight { get; set; }

        public CheckState LoBacCheckState { get { return (LoBac ?? false) ? CheckState.Checked : CheckState.Unchecked; } }
        public string LoBacString { get { return (LoBac ?? false) ? "x" : ""; } }
    }

    public class InventoryProductReturn
    {
        public string ProductKey { get; set; }
        public string ProductName { get; set; }
        public ProductTypeEnum? ProductType { get; set; }
        public string ProductSubType { get; set; }
        public string ProductCode { get; set; }
        public bool IsActive { get; set; }

        public string FullProductName { get { return string.IsNullOrWhiteSpace(ProductCode) ? ProductName : string.Format("{0} - {1}", ProductCode, ProductName); } }
    }

    public class PackagingProductReturn
    {
        public string ProductKey { get; set; }
        public string ProductName { get; set; }
        public string ProductNameFull { get; set; }
        public string ProductCode { get; set; }
        public bool IsActive { get; set; }
        public double Weight { get; set; }
        public double PackagingWeight { get; set; }
        public double PalletWeight { get; set; }
    }

    public class InventoryTreatmentReturn
    {
        public string TreatmentKey { get; set; }
        public string TreatmentName { get; set; }
        public string TreatmentNameShort { get; set; }
    }

    public class TitledShippingLabel : ShippingLabel
    {
        public string Title { get; set; }
    }

}