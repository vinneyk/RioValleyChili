using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseService
{
    public interface ICreateLocationParameters : ISaveLocationParameters
    {
        string FacilityKey { get; }

        LocationType LocationType { get; }
    }
}