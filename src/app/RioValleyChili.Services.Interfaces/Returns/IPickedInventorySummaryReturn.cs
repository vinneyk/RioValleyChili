namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IPickedInventorySummaryReturn
    {
        string PickedInventoryKey { get; }
        int TotalQuantityPicked { get; }
        double TotalWeightPicked { get; }
    }
}