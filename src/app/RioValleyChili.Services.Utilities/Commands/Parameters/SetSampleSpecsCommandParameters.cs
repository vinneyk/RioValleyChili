using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetSampleSpecsCommandParameters
    {
        internal ISetSampleSpecsParameters Parameters;

        internal SampleOrderItemKey SampleOrderItemKey;
    }
}