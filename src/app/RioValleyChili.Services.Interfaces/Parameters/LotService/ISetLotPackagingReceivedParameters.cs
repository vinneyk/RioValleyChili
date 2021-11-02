namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ISetLotPackagingReceivedParameters
    {
        string LotKey { get; }
        string ReceivedPackagingProductKey { get; }
    }
}