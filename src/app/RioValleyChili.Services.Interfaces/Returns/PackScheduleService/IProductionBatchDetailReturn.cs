using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionBatchDetailReturn : IProductionBatchSummaryReturn
    {
        string PackScheduleKey { get; }
        string ChileProductKey { get; }
        string ChileProductName { get; }

        IWorkTypeReturn WorkType { get; }
        INotebookReturn InstructionsNotebook { get; }
        IEnumerable<IChileProductAdditiveIngredientSummaryReturn> AdditiveIngredients { get; }
        IProductionBatchMaterialsSummaryReturn WipMaterialsSummary { get; }
        IProductionBatchMaterialsSummaryReturn FinishedGoodsMaterialsSummary { get; }
        IEnumerable<IProductionBatchPackagingMaterialSummaryReturn> PackagingMaterialSummaries { get; }
        IEnumerable<IPickedInventoryItemReturn> PickedInventoryItems { get; }
    }
}