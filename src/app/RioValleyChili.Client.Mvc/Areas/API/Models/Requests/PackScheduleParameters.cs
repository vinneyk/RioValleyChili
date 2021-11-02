using System;
using System.ComponentModel.DataAnnotations;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class CreatePackSchedule : PackScheduleParametersBase
    {
        public string CustomerKey { get; set; }
        public string OrderNumber { get; set; }

        public DateTime? DateCreated { get; set; }
        public int? Sequence { get; set; }
        public int? PSNum { get; set; }
    }

    public class UpdatePackSchedule : PackScheduleParametersBase
    {
        public string PackScheduleKey { get; internal set; }
        public string CustomerKey { get; set; }
        public string OrderNumber { get; set; }
    }

    public abstract class PackScheduleParametersBase 
    {
        [Range(0, Double.MaxValue)]
        public double BatchTargetWeight { get; set; }

        [Range(0, Double.MaxValue)]
        public double BatchTargetAsta { get; set; }

        [Range(0, Double.MaxValue)]
        public double BatchTargetScan { get; set; }

        [Range(0, Double.MaxValue)]
        public double BatchTargetScoville { get; set; }

        public DateTime? ProductionDeadline { get; set; }

        public DateTime ScheduledProductionDate { get; set; }

        [Required]
        public string WorkTypeKey { get; set; }

        [Required]
        public string ChileProductKey { get; set; }

        [Required]
        public string PackagingProductKey { get; set; }

        [Required]
        public string ProductionLineKey { get; set; }

        [StringLength(RioValleyChili.Core.Helpers.Constants.StringLengths.PackScheduleSummaryOfWork)]
        public string SummaryOfWork { get; set; }
    }
}