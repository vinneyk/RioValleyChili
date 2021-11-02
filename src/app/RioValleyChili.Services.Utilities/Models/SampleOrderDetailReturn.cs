using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderDetailReturn : ISampleOrderDetailReturn
    {
        public string SampleRequestKey { get { return SampleOrderKeyReturn.SampleOrderKey; } }
        public DateTime DateDue { get; internal set; }
        public DateTime DateReceived { get; internal set; }
        public DateTime? DateCompleted { get; internal set; }
        public SampleOrderStatus Status { get; internal set; }
        public bool Active { get; internal set; }
        public string FOB { get; internal set; }
        public string ShipVia { get; internal set; }
        public string Comments { get; internal set; }
        public string NotesToPrint { get; internal set; }

        public ICompanyHeaderReturn RequestedByCompany { get; internal set; }
        public ShippingLabel RequestedByShippingLabel { get; internal set; }

        public string ShipToCompany { get; internal set; }
        public ShippingLabel ShipToShippingLabel { get; internal set; }

        public ICompanyHeaderReturn Broker { get; internal set; }
        public IUserSummaryReturn CreatedByUser { get; internal set; }
        public IEnumerable<ISampleOrderItemReturn> Items { get; internal set; }
        public IEnumerable<ISampleOrderJournalEntryReturn> JournalEntries { get; internal set; }

        internal SampleOrderKeyReturn SampleOrderKeyReturn { get; set; }
    }
}