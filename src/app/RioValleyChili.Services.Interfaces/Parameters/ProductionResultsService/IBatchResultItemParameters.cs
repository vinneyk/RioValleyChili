namespace RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService
{
    public interface IBatchResultItemParameters
    {
        string PackagingKey { get; }
        string LocationKey { get; }
        string InventoryTreatmentKey { get; }
        int Quantity { get; }
    }
}