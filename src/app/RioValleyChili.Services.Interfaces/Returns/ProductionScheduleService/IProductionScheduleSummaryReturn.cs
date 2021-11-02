using System;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService
{
    public interface IProductionScheduleSummaryReturn
    {
        string ProductionScheduleKey { get; }
        DateTime ProductionDate { get; }
        ILocationReturn ProductionLine { get; }
    }
}
