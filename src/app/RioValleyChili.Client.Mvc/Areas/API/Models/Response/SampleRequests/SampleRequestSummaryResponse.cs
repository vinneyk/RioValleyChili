using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.SampleRequests
{
    public class SampleRequestSummaryResponse
    {
        public string SampleRequestKey { get; set; }
        public DateTime DateDue { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime? DateCompleted { get; set; }
        public SampleOrderStatus Status { get; set; }

        public CompanyResponse RequestedByCompany { get; set; }
        public CompanyResponse Broker { get; set; }
        public UserSummaryResponse CreatedByUser { get; set; }
    }
}