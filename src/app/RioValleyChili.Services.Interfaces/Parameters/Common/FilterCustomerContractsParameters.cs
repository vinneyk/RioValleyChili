using System;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class FilterCustomerContractsParameters
    {
        public string CustomerKey = null;

        public string BrokerKey = null;

        public ContractStatus? ContractStatus = null;

        public DateTime? TermBeginRangeStart = null;

        public DateTime? TermBeginRangeEnd = null;
    }
}