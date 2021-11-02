using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetLotHoldStatusParameters
    {
        internal ISetLotHoldStatusParameters Parameters { get; set; }

        internal LotKey LotKey { get; set; }
    }
}