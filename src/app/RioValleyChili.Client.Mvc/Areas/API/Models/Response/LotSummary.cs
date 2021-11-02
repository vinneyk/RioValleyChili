using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotSummary 
    {
        public string LotKey { get; set; }
        public LotHoldType? HoldType { get; set; }
        public string HoldDescription { get; set; }
        public LotQualityStatus QualityStatus { get; set; }
        public LotProductionStatus ProductionStatus { get; set; }
        public bool? LoBac { get; set; }
        public int? AstaCalc { get; set; }
        public DateTime LotDateCreated { get; set; }

        public InventoryProductResponse LotProduct { get; set; }
        public IEnumerable<LotAttribute> Attributes { get; set; }
        public IEnumerable<LotDefect> Defects { get; set; }
    }
}