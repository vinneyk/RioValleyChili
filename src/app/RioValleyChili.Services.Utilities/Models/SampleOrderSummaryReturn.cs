using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderSummaryReturn : ISampleOrderSummaryReturn
    {
        public string SampleRequestKey { get { return SampleOrderKeyReturn.SampleOrderKey; } }
        public DateTime DateDue { get; internal set; }
        public DateTime DateReceived { get; internal set; }
        public DateTime? DateCompleted { get; internal set; }
        public SampleOrderStatus Status { get; internal set; }

        public ICompanyHeaderReturn RequestedByCompany { get; internal set; }
        public ICompanyHeaderReturn Broker { get; internal set; }
        public IUserSummaryReturn CreatedByUser { get; internal set; }

        internal SampleOrderKeyReturn SampleOrderKeyReturn { get; set; }
    }
}