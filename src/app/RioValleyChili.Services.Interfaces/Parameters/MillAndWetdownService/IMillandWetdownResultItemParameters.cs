namespace RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService
{
    public interface IMillAndWetdownResultItemParameters
    {
        string LocationKey { get; }
        int Quantity { get; }
        string PackagingProductKey { get; }
    }
}