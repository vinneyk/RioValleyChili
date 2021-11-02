using System;

namespace RioValleyChili.Services.Models.Summaries
{
    public class ProductionScheduleItemSummary
    {
        public string PackScheduleKey { get; set; }

        public int OrderIndex { get; set; }

        public string ChileProductName { get; set; }

        public string PackagingName { get; set; }

        public int UnitsToProduce { get; set; }

        public int BatchWeight { get; set; }

        public int Granularity { get; set; }

        public int Asta { get; set; }

        public int Scan { get; set; }

        public int AOverB { get; set; }

        public DateTime DateDue { get; set; }

        public string PreInstruction { get; set; }

        public string PostInstruction { get; set; }
    }
}