using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class BatchProductionResultInventoryItemSummary : IBatchResultItemParameters
    {
        public string PackagingKey { get; set; }
        public string LocationKey { get; set; }
        public string InventoryTreatmentKey { get; set; }
        public int Quantity { get; set; }
    }
}