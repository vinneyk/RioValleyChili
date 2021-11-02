using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotBaseReturn
    {
        string LotKey { get; }
        LotHoldType? HoldType { get; }
        string HoldDescription { get; }
        LotQualityStatus QualityStatus { get; }
        LotProductionStatus ProductionStatus { get; }
        string Notes { get; }
    }
}