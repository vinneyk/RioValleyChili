using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetSampleMatchCommandParameters
    {
        internal ISetSampleMatchParameters Parameters;

        internal SampleOrderItemKey SampleOrderItemKey;
    }
}