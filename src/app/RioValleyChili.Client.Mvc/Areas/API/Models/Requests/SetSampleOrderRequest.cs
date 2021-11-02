using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetSampleOrderRequest
    {
        public DateTime? DateDue { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime? DateCompleted { get; set; }
        public SampleOrderStatus Status { get; set; }
        public bool Active { get; set; }
        public string Comments { get; set; }
        public string PrintNotes { get; set; }
        public double Volume { get; set; }
        public string BrokerKey { get; set; }
        public string RequestedByCompanyKey { get; set; }
        public ShippingLabel RequestedByShippingLabel { get; set; }
        public string ShipToCompany { get; set; }
        public ShippingLabel ShipToShippingLabel { get; set; }
        public string ShipVia { get; set; }
        public string FOB { get; set; }

        public IEnumerable<UpdateSampleItemRequest> Items { get; set; }
    }
}