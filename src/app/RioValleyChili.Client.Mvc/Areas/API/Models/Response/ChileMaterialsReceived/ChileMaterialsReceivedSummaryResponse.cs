using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.ChileMaterialsReceived
{
    public class ChileMaterialsReceivedSummaryResponse
    {
        public string LotKey { get; set; }

        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }
        public int TotalLoad { get; set; }

        public ChileProductResponse ChileProduct { get; set; }
        public CompanySummaryResponse Supplier { get; set; }
        public InventoryTreatmentResponse Treatment { get; set; }
    }
}