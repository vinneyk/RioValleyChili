using RioValleyChili.Core.Interfaces;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionBatchTargetParametersReturn : IProductionBatchTargetParameters
    {
        public double BatchTargetWeight { get; internal set; }
        public double BatchTargetAsta { get; internal set; }
        public double BatchTargetScan { get; internal set; }
        public double BatchTargetScoville { get; internal set; }
    }
}