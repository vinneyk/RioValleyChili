using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class ProductionBatchDTO
    {
        public string PackScheduleNumber { get; set; }

        public string LotNumber { get; set; }

        public DateTime DateCreated { get; set; }

        public int Sequence { get; set; }

        public int NumberOfPackagingUnits { get; set; }
    }
}