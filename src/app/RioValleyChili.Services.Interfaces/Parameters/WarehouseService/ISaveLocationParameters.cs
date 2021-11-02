using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseService
{
    public interface ISaveLocationParameters : IUserIdentifiable
    {
        string Description { get; }
        bool Active { get; }
        bool Locked { get; }
    }
}