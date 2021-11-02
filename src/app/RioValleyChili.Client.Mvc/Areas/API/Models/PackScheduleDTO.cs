using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class PackScheduleDTO
    {
        public string PackScheduleNumber { get; set; }

        public DateTime DateCreated { get; set; }

        public int SequentialNumber { get; set; }

        public DateTime ScheduledProductionDate { get; set; }

        public DateTime? ProductionDeadline { get; set; }

        public int ProductionLine { get; set; }

        public int WorkTypeId { get; set; }

        public string SummaryOfWork { get; set; }

        public string ChileProductKey { get; set; }

        public string ChileTypeKey { get; set; }

        public string PackagingProductKey { get; set; }

        public int Weight { get; set; }

        public int Asta { get; set; }

        public int Scoville { get; set; }

        public int Scan { get; set; }

        public int UnitsToProduce { get; set; }
    }
}