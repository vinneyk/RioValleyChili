using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderRequestReportReturn
    {
        string SampleOrderKey { get; }
        DateTime ShipByDate { get; }

        string Broker { get; }
        string ShipVia { get;  }
        string FOB { get; }
        string ShipToCompanyName  { get; }
        string RequestedByCompanyName { get; }

        ShippingLabel RequestedBy { get; }
        ShippingLabel ShipTo { get; }

        string SpecialInstructions { get; }

        IEnumerable<ISampleOrderRequestItemReportReturn> Items { get; }
    }

    public interface ISampleOrderRequestItemReportReturn
    {
        string ProductCode { get; }
        string ProductName { get; }
        string SampleMatch { get; }
        int Quantity { get; }
        string Description { get; }
    }
}