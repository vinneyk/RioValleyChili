namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ILotAllowanceParameters
    {
        string LotKey { get; }
        string ContractKey { get; }
        string CustomerOrderKey { get; }
        string CustomerKey { get; }
    }
}