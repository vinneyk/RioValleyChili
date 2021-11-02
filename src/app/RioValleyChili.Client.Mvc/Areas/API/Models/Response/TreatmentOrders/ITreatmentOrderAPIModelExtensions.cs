using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.TreatmentOrders
{
    public static class ITreatmentOrderAPIModelExtensions
    {
        public static string GetStatusDisplayText(this ITreatmentOrderAPIModel o)
        {
            if(o.OrderStatus == OrderStatus.Fulfilled)
            {
                return string.Format("Returned from Treatment {0:d/M/yyyy H:mm}", o.Returned);
            }

            if(o.ShipmentStatus == ShipmentStatus.Shipped)
            {
                return "Shipped";
            }

            return "Pending";
        }
    }
}