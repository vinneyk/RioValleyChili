using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.ChileMaterialsReceived
{
    public class ChileMaterialsReceivedDetailResponse
    {
        public string LotKey { get; set; }
        public ChileMaterialsReceivedType ChileMaterialsReceivedType { get; set; }
        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }

        public ChileProductResponse ChileProduct { get; set; }
        public CompanySummaryResponse Supplier { get; set; }
        public InventoryTreatmentResponse Treatment { get; set; }
        public IEnumerable<ChileMaterialsReceivedItemResponse> Items { get; set; }
        public ResourceLinkCollection Links { get; set; }

        public bool IsEditingEnabled { get; set; }
    }
}