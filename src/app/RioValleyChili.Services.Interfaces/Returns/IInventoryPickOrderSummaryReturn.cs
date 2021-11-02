namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IInventoryPickOrderSummaryReturn
    {
        string InventoryPickKey { get; }
        int TotalQuantity { get; }
        double TotalWeight { get; }
    }
}