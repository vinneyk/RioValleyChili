using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Models
{
    public class ProductionBatchMaterialsSummary : IChileProductAdditiveIngredientSummaryReturn, IChileProductIngredientKey
    {
        public string ChileProductIngredientKey { get { return new ChileProductIngredientKey(this).KeyValue; } }

        public ProductTypeEnum ProductType { get; set; }
        
        public LotTypeEnum LotType { get; set; }

        public string IngredientName { get; set; }

        public double TargetPercentage { get; set; }

        public double TargetWeight { get; set; }

        public double WeightPicked { get; set; }

        public double PercentOfPicked { get; set; }

        #region Implementation of IChileProductIngredientKey.

        public int ChileProductIngredientKey_ChileProductId { get; set; }

        public int ChileProductIngredientKey_AdditiveTypeId { get; set; }

        #endregion
    }
}