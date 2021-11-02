namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IBatchPickedInventoryItemReturn : IPickedInventoryItemReturn
    {
        string NewLotKey { get; }
    }
}