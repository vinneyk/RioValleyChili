using System;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class FilterSalesOrdersParameters
    {
        public string CustomerKey = null;
        public string BrokerKey = null;
        public SalesOrderStatus? SalesOrderStatus = null;
        public DateTime? OrderReceivedRangeStart = null;
        public DateTime? OrderReceivedRangeEnd = null;
        public DateTime? ScheduledShipDateRangeStart = null;
        public DateTime? ScheduledShipDateRangeEnd = null;
    }
}