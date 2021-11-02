using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class ResolveLotDefectParameters
    {
        internal IResolveLotDefectParameters Parameters { get; set; }

        internal LotDefectKey LotDefectKey { get; set; }
    }
}