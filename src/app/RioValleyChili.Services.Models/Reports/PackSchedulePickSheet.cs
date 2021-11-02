using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Models.Reports
{
    public class PackSchedulePickSheet
    {
        public PackSchedulePickSheetHeader Header { get; set; }

        public IEnumerable<PackSchedulePickSheetItem> PickSheetItems { get; set; }
    }

    public class PackSchedulePickSheetHeader
    {
        public string ProductName { get; set; }

        public string PackScheduleNumber { get; set; }

        public DateTime ScheduledProductionDate { get; set; }

        public string WorkType { get; set; }

        public string CustomerName { get; set; }

        public string SummmaryOfWork { get; set; }
    }

    public class PackSchedulePickSheetItem
    {
        public string Location { get; set; }

        public string LotNumber { get; set; }

        [Obsolete("Use ProductionBatchKey or ProductionBatchOutputLotNumber instead.")]
        public string BatchLotNumber { get; set; }

        public string ProductionBatchKey { get; set; }

        public string ProductionBatchOutputLotNumber { get; set; }

        public string ProductName { get; set; }

        public string PackagingName { get; set; }

        public bool LowBacteria { get; set; }

        public string TreatmentType { get; set; }

        public decimal Quantity { get; set; }

        public int Weight { get; set; }
    }
}