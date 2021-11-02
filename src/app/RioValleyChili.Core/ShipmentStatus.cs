namespace RioValleyChili.Core
{
    public enum ShipmentStatus
    {
        Unscheduled = 0,
        Scheduled = 1,
        Shipped = 10,
    }

    public static class ShipmentStatusExtensions
    {
        public static ShipmentStatus FromOrderStatID(int? orderStatID)
        {
            switch(orderStatID)
            {
                case 1: return ShipmentStatus.Scheduled;

                case 3:
                case 5: return ShipmentStatus.Shipped;

                default: return ShipmentStatus.Unscheduled;
            }
        }
    }
}