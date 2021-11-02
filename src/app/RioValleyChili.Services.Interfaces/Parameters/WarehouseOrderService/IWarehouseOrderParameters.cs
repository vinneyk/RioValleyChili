using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService
{
    public interface IWarehouseOrderParameters : IUserIdentifiable
    {
        string DestinationWarehouseKey { get; }
    }
}