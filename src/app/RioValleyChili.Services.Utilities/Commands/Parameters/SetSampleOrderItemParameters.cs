using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetSampleOrderItemParameters
    {
        internal ISampleOrderItemParameters Parameters;

        internal SampleOrderItemKey SampleOrderItemKey;
        internal ProductKey ProductKey;
        internal LotKey LotKey;
    }
}