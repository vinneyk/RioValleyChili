using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateProductionBatchParameters : IUpdateProductionBatchParameters
    {
        public string UserToken { get; set; }
        public string ProductionBatchKey { get; set; }
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }
        public string Notes { get; set; }
    }
}