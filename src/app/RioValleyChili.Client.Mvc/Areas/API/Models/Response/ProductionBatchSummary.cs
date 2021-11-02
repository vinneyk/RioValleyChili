namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class ProductionBatchSummary 
    {
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }
        public string ProductionBatchKey { get; set; }
        public string OutputLotKey { get; set; }
        public PackagingProductResponse PackagingProduct { get; set; }
        public bool HasProductionBeenCompleted { get; set; }
        public string Notes { get; set; }
    }
}