namespace RioValleyChili.Services.Interfaces.Returns.WarehouseService
{
    public interface IFacilitySummaryReturn
    {
        string FacilityKey { get; }
        string FacilityName { get; }
        bool Active { get; }
    }
}