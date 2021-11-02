using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionScheduleDetailReturn : IProductionScheduleDetailReturn
    {
        public string ProductionScheduleKey { get { return ProductionScheduleKeyReturn.ProductionScheduleKey; } }
        public DateTime ProductionDate { get; internal set; }
        public ILocationReturn ProductionLine { get; internal set; }
        public IEnumerable<IProductionScheduleItemReturn> ScheduledItems { get; set; }

        internal ProductionScheduleKeyReturn ProductionScheduleKeyReturn { get; set; }
    }

    internal class ProductionScheduleItemReturn : IProductionScheduleItemReturn
    {
        public int Index { get; set; }
        public bool FlushBefore { get; set; }
        public string FlushBeforeInstructions { get; set; }
        public bool FlushAfter { get; set; }
        public string FlushAfterInstructions { get; set; }
        public IScheduledPackScheduleReturn PackSchedule { get; set; }
    }

    internal class ScheduledPackScheduleReturn : IScheduledPackScheduleReturn
    {
        public string PackScheduleKey { get { return PackScheduleKeyReturn.PackScheduleKey; } }
        public DateTime? ProductionDeadline { get; set; }
        public string Instructions { get; set; }
        public double? AverageGranularity { get; set; }
        public double AverageAoverB { get; set; }
        public double AverageScoville { get; set; }
        public double AverageScan { get; set; }
        public IChileProductReturn ChileProduct { get; set; }

        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }
    }
}