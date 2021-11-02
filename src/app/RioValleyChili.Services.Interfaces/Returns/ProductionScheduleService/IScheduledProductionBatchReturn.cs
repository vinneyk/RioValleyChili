namespace RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService
{
    public interface IScheduledProductionBatchReturn
    {
        string ProductionBatchKey { get; }

        string OutputLotKey { get; }
    }
}