namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ICreateLotDefectReturn : ILotStatInfoReturn
    {
        string LotDefectKey { get; }
    }
}