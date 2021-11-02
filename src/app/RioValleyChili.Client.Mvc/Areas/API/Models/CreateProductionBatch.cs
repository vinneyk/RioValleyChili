using System;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateProductionBatchDto
    {
        public int NumberOfPackagingUnits { get; set; }

        public string PackScheduleKey { get; set; }

        [Range(0, double.MaxValue)]
        public double BatchTargetWeight { get; set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetAsta { get; set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetScan { get; set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetScoville { get; set; }

        public string Notes { get; set; }

        public string[] Instructions { get; set; }

        public LotTypeEnum? LotType { get; set; }
        public DateTime? LotDateCreated { get; set; }
        public int? LotSequence { get; set; }
    }
}