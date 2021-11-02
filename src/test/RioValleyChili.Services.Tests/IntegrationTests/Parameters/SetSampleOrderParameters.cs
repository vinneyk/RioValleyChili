using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetSampleOrderParameters : ISetSampleOrderParameters
    {
        public string UserToken { get; set; }
        public string SampleOrderKey { get; set; }
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
        public string ShipmentMethod { get; set; }
        public string FOB { get; set; }
        public IEnumerable<ISampleOrderItemParameters> Items { get; set; }
    }
}