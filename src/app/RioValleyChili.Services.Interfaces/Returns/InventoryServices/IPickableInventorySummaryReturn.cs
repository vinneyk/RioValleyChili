namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IPickableInventorySummaryReturn : IInventorySummaryReturn
    {
        bool ValidForPicking { get; }
    }
}