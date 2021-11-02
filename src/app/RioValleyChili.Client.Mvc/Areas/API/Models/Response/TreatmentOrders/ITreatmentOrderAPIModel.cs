using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.TreatmentOrders
{
    public interface ITreatmentOrderAPIModel
    {
        OrderStatus OrderStatus { get; }
        ShipmentStatus ShipmentStatus { get; }
        DateTime? Returned { get; }
    }
}