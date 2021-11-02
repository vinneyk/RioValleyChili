using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Models.Reports
{
    public class ProductionScheduleReport
    {
        public ProductionScheduleHeader ProductionScheduleHeader { get; set; }

        public IEnumerable<ProductionScheduleItem> ProductionScheduleItems { get; set; }
    }

    public class ProductionScheduleHeader
    {
        public DateTime ProductionDate { get; set; }

        public int LineNumber { get; set; }
    }

    public class ProductionScheduleItem
    {
        public string PreProductionInstruction { get; set; }

        public string ProductionInstructions { get; set; }

        public string PostProductionInstruction { get; set; }

        public string PackScheduleNumber { get; set; }

        public bool PackScheduleIsPartiallyProduced { get; set; }

        public string ProductDescription { get; set; }

        public string MinGranulation { get; set; }

        public string MaxGranulation { get; set; }

        public string Scan { get; set; }

        public string WorkType { get; set; }

        public string PackagingName { get; set; }

        public int PackagingTypeId { get; set; }

        public DateTime? ProductionDeadline { get; set; }

        public IEnumerable<string> BatchLotNumbers { get; set; }
    }
}
