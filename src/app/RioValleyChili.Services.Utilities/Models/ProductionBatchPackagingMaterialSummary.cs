using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    public class ProductionBatchPackagingMaterialSummary : IProductionBatchPackagingMaterialSummaryReturn
    {
        public string PackagingKey { get { return PackagingProductKeyReturn.ProductKey; } }

        public string PackagingDescription { get; set; }

        public int QuantityPicked { get; set; }

        public int TotalQuantityNeeded { get; set; }

        public int QuantityRemainingToPick { get { return TotalQuantityNeeded - QuantityPicked; } }

        public bool IsPickCompleted { get { return QuantityRemainingToPick == 0; } }

        #region Internal Parts

        internal ProductKeyReturn PackagingProductKeyReturn { get; set; }

        #endregion
    }
}