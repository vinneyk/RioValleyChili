using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ProductionBatchDetails
    {
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }
        public string ProductionBatchKey { get; set; }
        public string OutputLotKey { get; set; }
        public bool HasProductionBeenCompleted { get; set; }
        public string Notes { get; set; }
        public string PackScheduleKey { get; set; }
        public string ChileProductKey { get; set; }
        public string ChileProductName { get; set; }
        public WorkType WorkType { get; set; }
        public PackagingProductResponse PackagingProduct { get; set; }
        public IEnumerable<ChileProductAdditiveIngredientSummary> AdditiveIngredients { get; set; }
        public ProductionBatchMaterialsSummary WipMaterialsSummary { get; set; }
        public ProductionBatchMaterialsSummary FinishedGoodsMaterialsSummary { get; set; }
        public IEnumerable<ProductionBatchPackagingMaterialSummary> PackagingMaterialSummaries { get; set; }
        public IEnumerable<PickedInventoryItem> PickedInventoryItems { get; set; }
        public Notebook InstructionsNotebook { get; set; }
        public bool IsLocked { get; set; }
    }
}