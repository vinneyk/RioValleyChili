namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseService
{
    public interface IUpdateFacilityParameters : ICreateFacilityParameters
    {
        string FacilityKey { get; }
    }
}