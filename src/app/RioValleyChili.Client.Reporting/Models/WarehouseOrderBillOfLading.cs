using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class WarehouseOrderBillOfLading
    {
        public InventoryShipmentOrderTypeEnum OrderType { get; set; }
        public string OrderKey { get; set; }
        public int? MoveNum { get; set; }
        public int TotalQuantity { get; set; }
        public double PalletWeight { get; set; }
        public double TotalGrossWeight { get; set; }
        public double TotalNetWeight { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string SourceFacilityLabelName { get; set; }
        public AddressLabel ShipperAddress { get; set; }
        public ShipmentInformation ShipmentInformation { get; set; }
        public IEnumerable<PackingListPickedInventoryItem> Items { get; set; }
        
        public string TransferNumberLabel { get { return OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? "Order #:" : "Transfer #:"; } }
        public string ReportMoveNumber { get { return MoveNum == null ? "" : OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? MoveNum.Value.ToString() : MoveNum.Value.ToString("0000-000"); } }

        public FacilityAddressLabel ShipperFacilityAddress
        {
            get
            {
                if(_shipperFacilityAddress == null)
                {
                    _shipperFacilityAddress = new FacilityAddressLabel
                        {
                            FacilityName = OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? SourceFacilityLabelName : null,
                            AttentionLine = ShipperAddress.AttentionLine,
                            CompanyName = ShipperAddress.CompanyName,
                            AddressLine1 = ShipperAddress.AddressLine1,
                            AddressLine2 = ShipperAddress.AddressLine2,
                            AddressLine3 = ShipperAddress.AddressLine3,
                            Phone = ShipperAddress.Phone,
                            City = ShipperAddress.City,
                            State = ShipperAddress.State,
                            PostalCode = ShipperAddress.PostalCode,
                            Country = ShipperAddress.Country
                        };
                }

                return _shipperFacilityAddress;
            }
        }

        [Issue("Agreed with client that the BillOfLading and PackingList reports needed to display the user-supplied ShipmentInformation.PalletWeight" +
               "when defined and default to PalletQuantity * DefaultPalletWeight (60 as of time of writing). -RI 2016-09-27",
               References = new[] { "RVCADMIN-1318", "https://solutionhead.slack.com/archives/D03BRUYU4/p1475008918000133" })]
        public double PalletWeight_Access { get { return ShipmentInformation.PalletWeight ?? ShipmentInformation.PalletQuantity * Constants.Reporting.DefaultOrderPalletWeight; } }
        public double TotalGrossWeight_Access { get { return PalletWeight_Access + (Items.Select(i => i.GrossWeight).DefaultIfEmpty(0).Sum()); } }

        private FacilityAddressLabel _shipperFacilityAddress;
    }
}