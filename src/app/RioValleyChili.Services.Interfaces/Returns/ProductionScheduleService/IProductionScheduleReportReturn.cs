using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService
{
    public interface IProductionScheduleReportReturn
    {
        DateTime Timestamp { get; set; }
        DateTime ProductionDate { get; }
        string ProductionLocation { get; }
        IEnumerable<IProductionScheduleItemReportReturn> ScheduledItems { get; }
    }

    public interface IProductionScheduleItemReportReturn
    {
        bool FlushBefore { get; }
        string FlushBeforeInstructions { get; }

        string PackScheduleKey { get; }
        string ProductName { get; }
        string CustomerName { get; }
        string WorkType { get; }
        IEnumerable<IProductionScheduleBatchReturn> ProductionBatches { get; }

        double? Granularity { get; }
        double? Scan { get; }
        string Instructions { get; }
        DateTime? ProductionDeadline { get; }
        string OrderNumber { get; }
        string PackagingProduct { get; }

        bool FlushAfter { get; }
        string FlushAfterInstructions { get; }
    }

    public interface IProductionScheduleBatchReturn
    {
        string LotNumber { get; }
    }
}