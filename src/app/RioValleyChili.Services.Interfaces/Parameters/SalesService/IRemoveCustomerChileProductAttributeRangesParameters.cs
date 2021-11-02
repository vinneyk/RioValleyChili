namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface IRemoveCustomerChileProductAttributeRangesParameters
    {
        string CustomerKey { get; }
        string ChileProductKey { get; }
    }
}