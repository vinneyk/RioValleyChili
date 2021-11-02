using System;
using System.Collections.Generic;
using RioValleyChili.Services.Models.Summaries;

namespace RioValleyChili.Services.Models.Details
{
    public class ProductionScheduleDetails
    {
        public string ProductionScheduleKey { get; set; }

        public DateTime ProductionDate { get; set; }

        public string ProductionLine { get; set; }

        public IList<ProductionScheduleItemSummary> ProductionScheduleItems { get; set; }

        public string CreatedByUser { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateProduced { get; set; }
        
        //Roman: production schedules are editable if they have not been produced
        public bool IsEditable { get; set; }

        //Roman: for now, records can be deleted if they are editable
        public bool CanBeDeleted { get; set; }
    }
}
