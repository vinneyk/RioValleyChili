using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class PackScheduleSummary 
    {
        public string PackScheduleKey { get; set; }
        public int? PSNum { get; set; }
        public string WorkType { get; set; }
        public ProductionBatchTargets TargetParameters { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ScheduledProductionDate { get; set; }
        public DateTime? ProductionDeadline { get; set; }
        public string WorkTypeKey { get; set; }
        public string ChileProductKey { get; set; }
        public string ChileProductName { get; set; }
        public string ProductionLineDescription { get; set; }
        public string ProductionLineKey { get; set; }
        public string OrderNumber { get; set; }
        public CompanyResponse Customer { get; set; }
    }
}