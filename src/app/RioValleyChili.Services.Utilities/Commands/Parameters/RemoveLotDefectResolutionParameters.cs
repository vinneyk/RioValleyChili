using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class RemoveLotDefectResolutionParameters
    {
        internal IRemoveLotDefectResolutionParameters Parameters { get; set; }

        internal LotDefectKey LotDefectKey { get; set; }
    }
}