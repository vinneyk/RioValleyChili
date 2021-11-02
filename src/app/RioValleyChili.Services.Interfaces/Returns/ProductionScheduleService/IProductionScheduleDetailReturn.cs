using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService
{
    public interface IProductionScheduleDetailReturn
    {
        string ProductionScheduleKey { get; }
        DateTime ProductionDate { get; }
        ILocationReturn ProductionLine { get; }
        IEnumerable<IProductionScheduleItemReturn> ScheduledItems { get; }
    }

    public interface IProductionScheduleItemReturn
    {
        int Index { get; }
        bool FlushBefore { get; }
        string FlushBeforeInstructions { get; }
        bool FlushAfter { get; }
        string FlushAfterInstructions { get; }

        IScheduledPackScheduleReturn PackSchedule { get; }
    }

    public interface IScheduledPackScheduleReturn
    {
        string PackScheduleKey { get; }

        DateTime? ProductionDeadline { get; }
        string Instructions { get; }

        double? AverageGranularity { get; }
        double AverageAoverB { get; }
        double AverageScoville { get; }
        double AverageScan { get; }

        IChileProductReturn ChileProduct { get; }
    }
}