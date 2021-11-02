namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface IDehydratedInputReturn : IDehydratedMaterialsReceivedItemBaseReturn
    {
        string LotKey { get; }
        string DehydratorName { get; }
    }
}