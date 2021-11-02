using System;

namespace RioValleyChili.Core
{
    public enum tblOrderStatus
    {
        Scheduled = 1,
        Staged = 2,
        Shipped = 3,
        Invoiced = 4,
        Treated = 5,
        Void = 9
    }

    public static class tblOrderStatusExtensions
    {
        public static tblOrderStatus? ToTblOrderStatus(this int? tblOrderStatus)
        {
            switch(tblOrderStatus)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 9:
                    return (tblOrderStatus?) tblOrderStatus;
            }
            return null;
        }

        public static SalesOrderStatus ToCustomerOrderStatus(this tblOrderStatus status)
        {
            switch(status)
            {
                case tblOrderStatus.Scheduled:
                case tblOrderStatus.Staged:
                case tblOrderStatus.Shipped:
                case tblOrderStatus.Treated:
                case tblOrderStatus.Void:
                    return SalesOrderStatus.Ordered;

                case tblOrderStatus.Invoiced:
                    return SalesOrderStatus.Invoiced;

                default: throw new ArgumentOutOfRangeException("status");
            }
        }

        public static ShipmentStatus ToShipmentStatus(this tblOrderStatus status)
        {
            switch(status)
            {
                case tblOrderStatus.Scheduled:
                case tblOrderStatus.Staged:
                case tblOrderStatus.Treated:
                    return ShipmentStatus.Scheduled;

                case tblOrderStatus.Shipped:
                case tblOrderStatus.Invoiced:
                    return ShipmentStatus.Shipped;
                    
                case tblOrderStatus.Void:
                    return ShipmentStatus.Unscheduled;

                default: throw new ArgumentOutOfRangeException("status");
            }
        }
    }
}