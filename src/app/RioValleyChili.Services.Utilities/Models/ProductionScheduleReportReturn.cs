using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionScheduleReportReturn : IProductionScheduleReportReturn
    {
        public DateTime Timestamp { get; set; }
        public DateTime ProductionDate { get; internal set; }
        public string ProductionLocation { get; internal set; }
        public IEnumerable<IProductionScheduleItemReportReturn> ScheduledItems { get; internal set; }
    }

    internal class ProductionScheduleItemReportReturn : IProductionScheduleItemReportReturn
    {
        public bool FlushBefore { get; internal set; }
        public string FlushBeforeInstructions { get; internal set; }

        public string PackScheduleKey { get { return PackScheduleKeyReturn.PackScheduleKey; } }
        public string ProductName { get { return ChileProductReturn.ProductCodeAndName; } }
        public string CustomerName { get; internal set; }
        public string WorkType { get; set; }
        public IEnumerable<IProductionScheduleBatchReturn> ProductionBatches { get; internal set; }

        public double? Granularity { get; internal set; }
        public double? Scan { get; internal set; }
        public string Instructions { get; internal set; }
        public DateTime? ProductionDeadline { get; internal set; }
        public string OrderNumber { get; internal set; }

        public string PackagingProduct { get; internal set; }

        public bool FlushAfter { get; internal set; }
        public string FlushAfterInstructions { get; internal set; }

        internal ChileProductReturn ChileProductReturn { get; set; }
        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }
    }

    internal class ProductionScheduleBatchReturn : IProductionScheduleBatchReturn
    {
        public string LotNumber { get { return LotKeyReturn.LotKey; } }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }
}