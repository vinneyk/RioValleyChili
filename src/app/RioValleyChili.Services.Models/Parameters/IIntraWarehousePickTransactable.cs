using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Models.Parameters
{
    public interface IIntraWarehousePickTransactable : IUserIdentifiable
    {
        string WarehouseKey { get; }
    }
}