using System;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    public interface IShippingInstructions
    {
        DateTime? RequiredDeliveryDateTime { get; }
        DateTime? ShipmentDate { get; }

        string InternalNotes { get; }
        string ExternalNotes { get; }
        string SpecialInstructions { get; }

        ShippingLabel ShipFromOrSoldToShippingLabel { get; }
        ShippingLabel ShipToShippingLabel { get; }
        ShippingLabel FreightBillToShippingLabel { get; }
    }
}