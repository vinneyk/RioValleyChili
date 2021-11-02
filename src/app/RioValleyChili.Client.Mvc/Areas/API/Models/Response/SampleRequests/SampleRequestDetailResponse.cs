using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.SampleRequests
{
    public class SampleRequestDetailResponse
    {
        public string SampleRequestKey { get; set; }
        public DateTime DateDue { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime? DateCompleted { get; set; }
        public SampleOrderStatus Status { get; set; }
        public bool Active { get; set; }
        public string FOB { get; set; }
        public string ShipVia { get; set; }
        public string Comments { get; set; }
        public string NotesToPrint { get; set; }
        
        public CompanyResponse RequestedByCompany { get; set; }
        public ShippingLabel RequestedByShippingLabel { get; set; }
        
        public string ShipToCompany { get; set; }
        public ShippingLabel ShipToShippingLabel { get; set; }
        
        public CompanyResponse Broker { get; set; }
        public UserSummaryResponse CreatedByUser { get; set; }

        public IEnumerable<SampleRequestItemResponse> Items { get; set; }
        public IEnumerable<SampleRequestJournalEntryResponse> JournalEntries { get; set; }

        public ResourceLinkCollection Links { get; set; }
    }
}