using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ILotHold
    {
        LotHoldType HoldType { get; }

        string Description { get; }
    }
}