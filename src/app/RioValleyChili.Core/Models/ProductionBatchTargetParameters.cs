using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces;

namespace RioValleyChili.Core.Models
{
    [ComplexType]
    public class ProductionBatchTargetParameters : IProductionBatchTargetParameters
    {
        public virtual double BatchTargetWeight { get; set; }

        public virtual double BatchTargetAsta { get; set; }

        public virtual double BatchTargetScoville { get; set; }

        public virtual double BatchTargetScan { get; set; }

        public ProductionBatchTargetParameters() { }

        public ProductionBatchTargetParameters(IProductionBatchTargetParameters parameters)
        {
            BatchTargetWeight = parameters.BatchTargetWeight;
            BatchTargetAsta = parameters.BatchTargetAsta;
            BatchTargetScoville = parameters.BatchTargetScoville;
            BatchTargetScan = parameters.BatchTargetScan;
        }
    }
}