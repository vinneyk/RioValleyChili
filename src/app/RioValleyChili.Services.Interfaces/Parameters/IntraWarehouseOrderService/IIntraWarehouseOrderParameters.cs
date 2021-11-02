using System;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService
{
    public interface IIntraWarehouseOrderParameters : IUserIdentifiable
    {
        string OperatorName { get; }
        DateTime MovementDate { get; }
    }
}