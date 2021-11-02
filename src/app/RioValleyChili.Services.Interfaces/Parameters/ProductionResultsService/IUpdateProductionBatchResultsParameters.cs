namespace RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService
{
    public interface IUpdateProductionBatchResultsParameters : IProductionBatchResultsParameters
    {
        string ProductionResultKey { get; }
    }
}