using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ProductionBatchMaterialsSummary
    {
        public ProductTypeEnum InventoryType { get; set; }
        public LotTypeEnum LotType { get; set; }
        public string IngredientName { get; set; }
        public double TargetPercentage { get; set; }
        public double TargetWeight { get; set; }
        public double WeightPicked { get; set; }
    }
}