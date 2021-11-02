using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class PackScheduleDetails
    {
        public string PackScheduleKey { get; set; }
        public int? PSNum { get; set; }
        public string WorkType { get; set; }
        public string ProductionLineDescription { get; set; }
        public ProductionBatchTargets TargetParameters { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ScheduledProductionDate { get; set; }
        public DateTime? ProductionDeadline { get; set; }
        public string WorkTypeKey { get; set; }
        public string ChileProductKey { get; set; }
        public string ChileProductName { get; set; }
        public string ProductionLineKey { get; set; }
        public string OrderNumber { get; set; }
        public CompanyResponse Customer { get; set; }
        public string PackagingProductKey { get; set; }
        public string PackagingProductName { get; set; }
        public double PackagingWeight { get; set; }
        public string SummaryOfWork { get; set; }
        public IEnumerable<ProductionBatchSummary> ProductionBatches { get; set; }
    }
}