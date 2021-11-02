namespace RioValleyChili.Core.Interfaces
{
    public interface IProductionBatchTargetParameters
    {
        double BatchTargetWeight { get; }
        double BatchTargetAsta { get; }
        double BatchTargetScan { get; }
        double BatchTargetScoville { get; }
    }
}