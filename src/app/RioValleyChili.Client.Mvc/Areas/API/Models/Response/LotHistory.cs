using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotHistoryResponse
    {
        public string LotKey { get; set; }
        public DateTime Timestamp { get; set; }
        public UserSummaryResponse Employee { get; set; }

        public bool LoBac { get; set; }
        public LotHoldType? HoldType { get; set; }
        public string HoldDescription { get; set; }
        public LotQualityStatus QualityStatus { get; set; }
        public LotProductionStatus ProductionStatus { get; set; }

        public InventoryProductResponse Product { get; set; }
        public IEnumerable<LotHistoryAttributeResponse> Attributes { get; set; }
        public IEnumerable<LotHistoryRecordResponse> History { get; set; }
    }

    public class LotHistoryAttributeResponse
    {
        public string AttributeShortName { get; set; }
        public double Value { get; set; }
        public DateTime AttributeDate { get; set; }
        public bool Computed { get; set; }
    }

    public class LotHistoryRecordResponse
    {
        public DateTime Timestamp { get; set; }
        public UserSummaryResponse Employee { get; set; }

        public bool LoBac { get; set; }
        public LotHoldType? HoldType { get; set; }
        public string HoldDescription { get; set; }
        public LotQualityStatus QualityStatus { get; set; }
        public LotProductionStatus ProductionStatus { get; set; }

        public IEnumerable<LotHistoryAttributeResponse> Attributes { get; set; }
    }
}