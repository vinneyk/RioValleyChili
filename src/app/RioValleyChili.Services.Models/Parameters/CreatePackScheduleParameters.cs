using System;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreatePackScheduleParameters : ICreatePackScheduleParameters
    {
        public string UserToken { get; set; }
        public string WorkTypeKey { get; set; }
        public string ChileProductKey { get; set; }
        public string PackagingProductKey { get; set; }
        public DateTime ScheduledProductionDate { get; set; }
        public DateTime? ProductionDeadline { get; set; }
        public string ProductionLineKey { get; set; }
        public string SummaryOfWork { get; set; }

        public string CustomerKey { get; set; }
        public string OrderNumber { get; set; }
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }

        public DateTime? DateCreated { get; set; }
        public int? Sequence { get; set; }
        public int? PSNum { get; set; }
    }
}