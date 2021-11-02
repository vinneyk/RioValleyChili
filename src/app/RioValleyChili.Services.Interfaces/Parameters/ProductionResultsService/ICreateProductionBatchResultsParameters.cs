namespace RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService
{
    public interface ICreateProductionBatchResultsParameters : IProductionBatchResultsParameters
    {
        string ProductionBatchKey { get; }
    }
}