using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Client.Mvc.Utilities.Translators;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotQualitySummaryResponse
    {
        public string LotKey { get; set; }
        public int? AstaCalc { get; set; }
        public bool? LoBac { get; set; }
        public LotHoldType? HoldType { get; set; }
        public LotQualityStatus QualityStatus { get; set; }
        public bool ProductSpecComplete { get; set; }
        public bool ProductSpecOutOfRange { get; set; }
        public string HoldDescription { get; set; }
        public DateTime LotDate { get; set; }
        public string Notes { get; set; }
        public LotProductionStatus ProductionStatus { get; set; }
        public InventoryProductResponse Product { get; set; }
        public IEnumerable<LotAttribute> Attributes { get; set; }
        public IEnumerable<LotDefect> Defects { get; set; }
        public string CustomerName { get; set; }
        public string CustomerKey { get; set; }
        public string OldContextLotStat { get; set; }

        public IEnumerable<LotQualityStatus> ValidLotQualityStatuses { get; set; }
        public IEnumerable<LotCustomerAllowanceResponse> CustomerAllowances { get; set; }
        public IEnumerable<LotCustomerOrderAllowanceResponse> CustomerOrderAllowances { get; set; }
        public IEnumerable<LotContractAllowanceResponse> ContractAllowances { get; set; }

        public string ProductionStatusDescription { get { return ProductionStatus.GetDescription(); } }
    }
}