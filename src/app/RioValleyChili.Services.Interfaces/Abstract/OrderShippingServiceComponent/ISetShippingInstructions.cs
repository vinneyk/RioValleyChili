using System;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface ISetShippingInstructions
    {
        DateTime? RequiredDeliveryDateTime { get; }
        DateTime? ShipmentDate { get; }

        string InternalNotes { get; }
        string ExternalNotes { get; }
        string SpecialInstructions { get; }

        ShippingLabel ShipFromOrSoldTo { get; }
        ShippingLabel ShipTo { get; }
        ShippingLabel FreightBillTo { get; }
    }
}