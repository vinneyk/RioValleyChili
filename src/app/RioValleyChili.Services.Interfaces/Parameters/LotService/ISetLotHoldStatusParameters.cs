using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ISetLotHoldStatusParameters : IUserIdentifiable
    {
        string LotKey { get; }

        ILotHold Hold { get; }
    }
}
