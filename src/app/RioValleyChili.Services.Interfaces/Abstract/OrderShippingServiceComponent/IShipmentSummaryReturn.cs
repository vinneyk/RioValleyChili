using System;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface IShipmentSummaryReturn
    {
        string ShipmentKey { get; }
        ShipmentStatus Status { get; }
        DateTime? ShipmentDate { get; }
    }
}