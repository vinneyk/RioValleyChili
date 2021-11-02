using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ProductionBatchPackagingMaterialSummary
    {
        public LotTypeEnum LotType { get { return LotTypeEnum.Packaging; } }
        public ProductTypeEnum InventoryType { get { return ProductTypeEnum.Packaging; } }
        public string PackagingKey { get; set; }
        public string PackagingDescription { get; set; }
        public int QuantityPicked { get; set; }
        public int TotalQuantityNeeded { get; set; }
        public int QuantityRemainingToPick { get; set; }
        public bool IsPickCompleted { get; set; }
    }
}