using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionBatchForPickingReturn : IProductionBatchSummaryReturn
    {
        string PackScheduleKey { get; }

        IChileProductDetailReturn ChileProductDetails { get; }

        IPickedInventoryDetailReturn PickedInventory { get; }
    }
}