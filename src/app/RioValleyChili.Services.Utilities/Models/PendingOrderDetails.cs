using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PendingOrderDetails : IPendingOrderDetails
    {
        public DateTime Now { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public DateTime NewStartDate { get; internal set; }
        public DateTime NewEndDate { get; internal set; }

        public int ScheduledAmount { get; internal set; }
        public int ShippedAmount { get; internal set; }
        public int RemainingAmount { get; internal set; }
        public int NewAmount { get; internal set; }

        public IEnumerable<IPendingCustomerOrderDetail> PendingCustomerOrders { get; internal set; }
        public IEnumerable<IPendingWarehouseOrderDetail> PendingWarehouseOrders { get; internal set; }
    }

    internal class PendingCustomerOrderDetail : IPendingCustomerOrderDetail
    {
        public string Name { get; set; }
        public DateTime? DateRecd { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string OrderNum { get { return MoveNum == null ? "" : MoveNum.Value.ToString(); } }
        public string Origin { get; set; }
        public OrderStatus Status { get; set; }
        public bool Sample { get; set; }

        public IEnumerable<IPendingOrderItem> Items { get; set; }

        internal int? MoveNum { get; set; }
    }

    internal class PendingOrderItem : IPendingOrderItem
    {
        public int QuantityPicked { get; internal set; }
        public int QuantityOrdered { get; internal set; }
        public string Packaging { get { return PackagingProduct.ProductName; } }
        public string Product { get; internal set; }
        public string Treatment { get; internal set; }
        public int LbsToShip { get { return (int) (QuantityOrdered * PackagingProduct.Weight); } }

        internal PackagingProductReturn PackagingProduct { get; set; }
    }

    internal class PendingWarehouseOrderDetail : IPendingWarehouseOrderDetail
    {
        public string From { get; internal set; }
        public string To { get; internal set; }
        public DateTime? DateRecd { get; internal set; }
        public DateTime? ShipmentDate { get; internal set; }
        public string OrderNum { get { return MoveNum == null ? "" : string.Format("{0:####-###}", MoveNum); } }
        public OrderStatus Status { get; internal set; }
        public IEnumerable<IPendingOrderItem> Items { get; internal set; }

        internal int? MoveNum { get; set; }
        internal IEnumerable<PickedInventoryItemSelect> PickedItemSelect { get; set; }

        internal void Initialize()
        {
            var pickedItems = PickedItemSelect.GroupBy(i => new
                {
                    productKey = i.LotSelect.ProductKey,
                    packagingKey = new PackagingProductKey(i.Item),
                    treatmentKey = new InventoryTreatmentKey(i.Item)
                }, i => i.Item.Quantity)
                .ToDictionary(g => g.Key, g => g.Sum());

            foreach(var item in Items.Cast<PendingPickOrderItem>())
            {
                var productKey = new ProductKey((IProductKey)item.DataModel);
                var packagingKey = new PackagingProductKey(item.DataModel);
                var treatmentKey = new InventoryTreatmentKey(item.DataModel);

                var noMatch = false;
                while(item.QuantityPicked < item.QuantityOrdered && !noMatch)
                {
                    var key = pickedItems.Keys.FirstOrDefault(k => k.productKey.Equals(productKey) && k.packagingKey.Equals(packagingKey) && k.treatmentKey.Equals(treatmentKey));
                    if(key == null)
                    {
                        noMatch = true;
                    }
                    else
                    {
                        var pick = Math.Min(pickedItems[key], item.QuantityOrdered - item.QuantityPicked);
                        item.QuantityPicked += pick;
                        pickedItems[key] -= pick;
                        if(pickedItems[key] == 0)
                        {
                            pickedItems.Remove(key);
                        }
                    }
                }
            }
        }
    }

    internal class PendingPickOrderItem : PendingOrderItem
    {
        internal InventoryPickOrderItem DataModel { get; set; }
    }

    internal class PickedInventoryItemSelect
    {
        internal PickedInventoryItem Item { get; set; }
        internal LotSelect LotSelect { get; set; }
    }

    internal class LotSelect
    {
        internal Lot Lot { get; set; }
        internal ChileLot ChileLot { get; set; }
        internal AdditiveLot AdditiveLot { get; set; }
        internal PackagingLot PackagingLot { get; set; }

        internal ProductKey ProductKey
        {
            get
            {
                if(ChileLot != null)
                {
                    return new ProductKey(ChileLot);
                }
                if(AdditiveLot != null)
                {
                    return new ProductKey(AdditiveLot);
                }
                if(PackagingLot != null)
                {
                    return new ProductKey(PackagingLot);
                }
                throw new ArgumentOutOfRangeException("Uknown type of lot.");
            }
        }
    }
}