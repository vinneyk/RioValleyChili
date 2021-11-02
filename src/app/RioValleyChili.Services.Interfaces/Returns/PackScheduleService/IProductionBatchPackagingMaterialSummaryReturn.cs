namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionBatchPackagingMaterialSummaryReturn
    {
        string PackagingKey { get; }

        string PackagingDescription { get; }

        int QuantityPicked { get; }

        int TotalQuantityNeeded { get; }

        int QuantityRemainingToPick { get; }

        bool IsPickCompleted { get; }
    }
}