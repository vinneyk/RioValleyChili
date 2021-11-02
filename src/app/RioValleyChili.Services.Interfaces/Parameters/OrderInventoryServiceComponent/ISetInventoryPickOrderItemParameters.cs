namespace RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent
{
    public interface ISetInventoryPickOrderItemParameters
    {
        string ProductKey { get; }
        string PackagingKey { get; }
        string TreatmentKey { get; }
        string CustomerKey { get; }
        int Quantity { get; }
        string CustomerProductCode { get; }
        string CustomerLotCode { get; }
    }
}