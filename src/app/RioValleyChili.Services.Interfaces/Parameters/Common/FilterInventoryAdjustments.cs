using System;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class FilterInventoryAdjustmentParameters
    {
        public string LotKey { get; set; }
        public DateTime? AdjustmentDateRangeStart { get; set; }
        public DateTime? AdjustmentDateRangeEnd { get; set; }
    }
}