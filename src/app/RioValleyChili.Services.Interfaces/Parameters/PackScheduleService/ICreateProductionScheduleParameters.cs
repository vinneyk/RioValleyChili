using System;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface ICreateProductionScheduleParameters : IUserIdentifiable
    {
        DateTime ProductionDate { get; }
        string ProductionLineLocationKey { get; }
    }
}