namespace RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService
{
    public interface ICreateChileMaterialsReceivedItemParameters
    {
        string GrowerCode { get; set; }
        string ToteKey { get; }
        int Quantity { get; }
        string PackagingProductKey { get; }
        string Variety { get; }
        string LocationKey { get; }
    }
}