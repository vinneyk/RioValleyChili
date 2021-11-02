using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IProductionScheduleKey : ILocationKey
    {
        DateTime ProductionScheduleKey_ProductionDate { get;  }
    }
}