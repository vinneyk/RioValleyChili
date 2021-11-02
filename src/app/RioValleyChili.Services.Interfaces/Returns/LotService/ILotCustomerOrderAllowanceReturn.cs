namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotCustomerOrderAllowanceReturn
    {
        string OrderKey { get; }
        int? OrderNumber { get; }
        string CustomerKey { get; }
        string CustomerName { get; }
    }
}