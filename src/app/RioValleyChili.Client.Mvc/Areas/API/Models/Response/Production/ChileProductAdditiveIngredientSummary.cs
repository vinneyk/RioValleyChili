using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ChileProductAdditiveIngredientSummary
    {
        public ProductTypeEnum InventoryType { get { return ProductTypeEnum.Additive; } }
        public LotTypeEnum LotType { get { return LotTypeEnum.Additive; } }
        public string IngredientName { get; set; }
        public double TargetPercentage { get; set; }
        public double TargetWeight { get; set; }
        public double WeightPicked { get; set; }
        public string ChileProductIngredientKey { get; set; }
    }
}