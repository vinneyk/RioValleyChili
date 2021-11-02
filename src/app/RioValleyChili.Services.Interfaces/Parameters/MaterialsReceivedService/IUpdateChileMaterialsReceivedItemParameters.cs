namespace RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService
{
    public interface IUpdateChileMaterialsReceivedItemParameters
    {
        string ItemKey { get; set; }
        string GrowerCode { get; set; }
        string ToteKey { get; }
        int Quantity { get; }
        string PackagingProductKey { get; }
        string Variety { get; }
        string LocationKey { get; }
    }
}