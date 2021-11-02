namespace RioValleyChili.Services.Interfaces.Parameters.SampleOrderService
{
    public interface ISampleOrderItemParameters
    {
        string SampleOrderItemKey { get; }
        int Quantity { get; }
        string Description { get; }
        string CustomerProductName { get; }
        string ProductKey { get; }
        string LotKey { get; }
    }
}