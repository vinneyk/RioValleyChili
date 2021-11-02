using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production
{
    public class ProductionScheduleDetailResponse
    {
        public string ProductionScheduleKey { get; set; }
        public DateTime ProductionDate { get; set; }
        public FacilityLocationResponse ProductionLine { get; set; }

        public IEnumerable<ProductionScheduleItemResponse> ScheduledItems { get; set; }

        public ResourceLinkCollection Links { get; set; }
    }

    public class ProductionScheduleItemResponse
    {
        public int Index { get; set; }
        public bool FlushBefore { get; set; }
        public string FlushBeforeInstructions { get; set; }
        public bool FlushAfter { get; set; }
        public string FlushAfterInstructions { get; set; }

        public ScheduledPackScheduleResponse PackSchedule { get; set; }
    }

    public class ScheduledPackScheduleResponse
    {
        public string PackScheduleKey { get; set; }

        public DateTime? ProductionDeadline { get; set; }
        public string Instructions { get; set; }

        public double? AverageGranularity { get; set; }
        public double AverageAoverB { get; set; }
        public double AverageScoville { get; set; }
        public double AverageScan { get; set; }

        public ChileProductResponse ChileProduct { get; set; }
    }
}