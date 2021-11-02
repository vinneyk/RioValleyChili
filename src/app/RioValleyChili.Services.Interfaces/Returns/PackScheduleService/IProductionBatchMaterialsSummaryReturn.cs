using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionBatchMaterialsSummaryReturn
    {
        ProductTypeEnum ProductType { get; }

        LotTypeEnum LotType { get; }

        string IngredientName { get; }

        double TargetPercentage { get; }

        double TargetWeight { get; }

        double WeightPicked { get; }
    }
}