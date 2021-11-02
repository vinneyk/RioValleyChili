using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetLotStatusParameters
    {
        internal ISetLotStatusParameters Parameters { get; set; }

        internal LotKey LotKey { get; set; }
    }
}