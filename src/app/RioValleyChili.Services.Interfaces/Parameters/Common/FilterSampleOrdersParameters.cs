using System;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class FilterSampleOrdersParameters
    {
        public DateTime? DateReceivedStart = null;
        public DateTime? DateReceivedEnd = null;
        public DateTime? DateCompletedStart = null;
        public DateTime? DateCompletedEnd = null;
        public SampleOrderStatus? Status = null;
        public string RequestedCompanyKey = null;
        public string BrokerKey = null;
    }
}