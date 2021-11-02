using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SampleOrderService
{
    public interface ISetSampleOrderParameters : IUserIdentifiable
    {
        string SampleOrderKey { get; }

        DateTime? DateDue { get; }
        DateTime DateReceived { get; }
        DateTime? DateCompleted { get; }

        SampleOrderStatus Status { get; }
        bool Active { get; }

        string Comments { get; }
        string PrintNotes { get; }
        double Volume { get; }

        string BrokerKey { get; }
        string RequestedByCompanyKey { get; }
        ShippingLabel RequestedByShippingLabel { get; }
        string ShipToCompany { get; }
        ShippingLabel ShipToShippingLabel { get; }

        string ShipmentMethod { get; }
        string FOB { get; }

        IEnumerable<ISampleOrderItemParameters> Items { get; }
    }
}