using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderDetailReturn
    {
        string SampleRequestKey { get; }
        DateTime DateDue { get; }
        DateTime DateReceived { get; }
        DateTime? DateCompleted { get; }
        SampleOrderStatus Status { get; }
        bool Active { get; }
        string FOB { get; }
        string ShipVia { get; }
        string Comments { get; }
        string NotesToPrint { get; }

        ICompanyHeaderReturn RequestedByCompany { get; }
        ShippingLabel RequestedByShippingLabel { get; }

        string ShipToCompany { get; }
        ShippingLabel ShipToShippingLabel { get; }

        ICompanyHeaderReturn Broker { get; }
        IUserSummaryReturn CreatedByUser { get; }

        IEnumerable<ISampleOrderItemReturn> Items { get; }
        IEnumerable<ISampleOrderJournalEntryReturn> JournalEntries { get; }
    }
}