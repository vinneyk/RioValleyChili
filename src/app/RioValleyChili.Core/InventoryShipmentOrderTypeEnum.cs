using System;

namespace RioValleyChili.Core
{
    public enum InventoryShipmentOrderTypeEnum : short
    {
        InterWarehouseOrder,
        TreatmentOrder,
        SalesOrder,
        ConsignmentOrder,
        MiscellaneousOrder
    }

    public static class InventoryShipmentOrderTypeEnumExtensions
    {
        public static TransType ToTransType(this InventoryShipmentOrderTypeEnum orderType)
        {
            switch(orderType)
            {
                case InventoryShipmentOrderTypeEnum.InterWarehouseOrder: return TransType.Move;
                case InventoryShipmentOrderTypeEnum.TreatmentOrder: return TransType.ToTrmt;
                case InventoryShipmentOrderTypeEnum.SalesOrder: return TransType.Sale;
                case InventoryShipmentOrderTypeEnum.ConsignmentOrder: return TransType.OnConsignment;
                case InventoryShipmentOrderTypeEnum.MiscellaneousOrder: return TransType.MiscInvoice;

                default: throw new ArgumentOutOfRangeException("orderType");
            }
        }

        public static InventoryShipmentOrderTypeEnum ToOrderType(this TransType transType)
        {
            switch(transType)
            {
                case TransType.Move: return InventoryShipmentOrderTypeEnum.InterWarehouseOrder;
                case TransType.ToTrmt: return InventoryShipmentOrderTypeEnum.TreatmentOrder;
                case TransType.Sale: return InventoryShipmentOrderTypeEnum.SalesOrder;
                case TransType.OnConsignment: return InventoryShipmentOrderTypeEnum.ConsignmentOrder;
                case TransType.MiscInvoice: return InventoryShipmentOrderTypeEnum.MiscellaneousOrder;

                default: throw new ArgumentOutOfRangeException("transType");
            }
        }
    }
}