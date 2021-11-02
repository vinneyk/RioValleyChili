// ReSharper disable RedundantExtendsListEntry

using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionBatchSummaryReturn : IProductionBatchSummaryReturn
    {
        public string ProductionBatchKey { get { return OutputLotKeyReturn.LotKey; } }

        public string OutputLotKey { get { return OutputLotKeyReturn.LotKey; } }

        public double BatchTargetWeight { get; internal set; }

        public double BatchTargetAsta { get; internal set; }

        public double BatchTargetScan { get; internal set; }

        public double BatchTargetScoville { get; internal set; }

        public IPackagingProductReturn PackagingProduct { get; internal set; }

        public bool HasProductionBeenCompleted { get; internal set; }

        public string Notes { get; internal set; }

        #region Internal Parts

        internal LotKeyReturn OutputLotKeyReturn { get; set; }

        #endregion
    }

    internal class ProductionBatchDetailReturn : ProductionBatchSummaryReturn, IProductionBatchDetailReturn
    {
        public string PackScheduleKey { get { return PackScheduleKeyReturn.PackScheduleKey; } }

        public string ChileProductKey { get { return ChileProductKeyReturn.ProductKey; } }

        public string ChileProductName { get; internal set; }

        public IWorkTypeReturn WorkType { get; internal set; }

        public INotebookReturn InstructionsNotebook { get; internal set; }

        public IEnumerable<IChileProductAdditiveIngredientSummaryReturn> AdditiveIngredients { get; internal set; }

        public IProductionBatchMaterialsSummaryReturn WipMaterialsSummary { get; internal set; }

        public IProductionBatchMaterialsSummaryReturn FinishedGoodsMaterialsSummary { get; internal set; }

        public IEnumerable<IProductionBatchPackagingMaterialSummaryReturn> PackagingMaterialSummaries { get; internal set; }

        public IEnumerable<IPickedInventoryItemReturn> PickedInventoryItems { get; internal set; }

        #region Internal Parts

        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }

        internal ProductKeyReturn ChileProductKeyReturn { get; set; }

        internal ChileProductWithIngredients ChileProductWithIngredients { get; set; }

        internal IEnumerable<PickedAdditiveInventoryItem> PickedAdditiveItems { get; set; }

        internal IEnumerable<PickedChileInventoryItem> PickedChileItems { get; set; }

        internal IEnumerable<PickedPackagingInventoryItem> PickedPackagingItems { get; set; }

        #endregion
    }

    internal class ProductionBatchForPickingReturn : ProductionBatchSummaryReturn, IProductionBatchForPickingReturn
    {
        public string PackScheduleKey { get { return PackScheduleKeyReturn.PackScheduleKey; } }

        public IChileProductDetailReturn ChileProductDetails { get; internal set; }

        public IPickedInventoryDetailReturn PickedInventory { get; internal set; }

        #region Internal Parts

        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }

        #endregion
    }
}

// ReSharper restore RedundantExtendsListEntry