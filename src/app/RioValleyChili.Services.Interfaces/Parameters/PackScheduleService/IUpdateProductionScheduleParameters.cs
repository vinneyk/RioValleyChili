using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface IUpdateProductionScheduleParameters : IUserIdentifiable
    {
        string ProductionScheduleKey { get; }
        IEnumerable<ISetProductionScheduleItemParameters> ScheduledItems { get; }
    }
}