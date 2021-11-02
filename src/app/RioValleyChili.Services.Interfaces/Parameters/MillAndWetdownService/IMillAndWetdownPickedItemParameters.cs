namespace RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService
{
    public interface IMillAndWetdownPickedItemParameters
    {
        string InventoryKey { get; }
        int Quantity { get; }
    }
}