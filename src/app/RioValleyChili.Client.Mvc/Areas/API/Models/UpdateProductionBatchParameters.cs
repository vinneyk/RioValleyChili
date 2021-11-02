using System.ComponentModel.DataAnnotations;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class UpdateProductionBatchParameters : IUpdateProductionBatchParameters
    {
        public string ProductionBatchKey { get; internal set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetWeight { get; set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetAsta { get; set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetScan { get; set; }
        
        [Range(0, double.MaxValue)]
        public double BatchTargetScoville { get; set; }

        public string Notes { get; set; }

        string IUserIdentifiable.UserToken { get; set; }
    }
}