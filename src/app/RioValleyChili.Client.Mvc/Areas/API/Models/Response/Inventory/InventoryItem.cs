using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class GetInventoryWithTotalsResponse
    {
        /// <summary>
        /// Contains the total pounds of inventory matching the supplied filter parameters. This total is for all matching inventory and is not limited to the inventory included in the paged data.
        /// </summary>
        public double TotalPounds { get; set; }
        public IEnumerable<InventoryItem> Items { get; set; }
    }
    public class InventoryItem
    {
        public string InventoryKey { get; set; }
        public string LotKey { get; set; }
        public string ToteKey { get; set; }
        public DateTime LotDateCreated { get; set; }
        public LotQualityStatus LotStatus { get; set; }
        public LotProductionStatus LotProductionStatus { get; set; }
        public int Quantity { get; set; }
        public LotHoldType? HoldType { get; set; }
        public int? AstaCalc { get; set; }
        public bool? LoBac { get; set; }
        public string HoldDescription { get; set; }
        public string ReceivedPackagingName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerKey { get; set; }
        public string Notes { get; set; }

        public InventoryProductResponse Product { get; set; }
        public PackagingProductResponse PackagingProduct { get; set; }
        public FacilityLocationResponse Location { get; set; }
        public InventoryTreatmentResponse InventoryTreatment { get; set; }
        public IEnumerable<LotAttribute> Attributes { get; set; }
        public IEnumerable<LotDefect> Defects { get; set; }

        public void Initialize(DateTime astaCalcDate)
        {
            if(AstaCalc != null)
            {
                var attributes = Attributes.ToList();
                attributes.Add(new LotAttribute
                    {
                        Key = "AstaC",
                        Name = "AstaC",
                        Value = AstaCalc.Value,
                        AttributeDate = astaCalcDate.ToShortDateString(),
                        Computed = true
                    });
                Attributes = attributes;
            }
        }
    }
}