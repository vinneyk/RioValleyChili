namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IChileProductAdditiveIngredientSummaryReturn : IProductionBatchMaterialsSummaryReturn
    {
        string ChileProductIngredientKey { get; }
    }
}