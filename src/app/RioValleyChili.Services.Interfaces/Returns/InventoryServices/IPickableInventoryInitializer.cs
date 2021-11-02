namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IPickableInventoryInitializer
    {
        void Initialize(IPickableInventorySummaryReturn inventory);
    }
}