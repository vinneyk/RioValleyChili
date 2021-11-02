namespace RioValleyChili.Core
{
    public enum OrderStatus : short
    {
        Scheduled = 0,
        Fulfilled,
        Void
    }

    public static class OrderStatusExtensions
    {
        public static OrderStatus ToOrderStatus(this tblOrderStatus? orderStatus)
        {
            switch(orderStatus)
            {
                case tblOrderStatus.Shipped:
                case tblOrderStatus.Treated:
                    return OrderStatus.Fulfilled;

                case tblOrderStatus.Void:
                    return OrderStatus.Void;
            }

            return OrderStatus.Scheduled;
        }
    }
}