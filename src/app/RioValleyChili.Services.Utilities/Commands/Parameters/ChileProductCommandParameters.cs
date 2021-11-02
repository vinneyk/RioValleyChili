using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class ChileProductCommandParameters
    {
        internal IChileProductParameters Parameters { get; set; }

        internal ChileTypeKey ChileTypeKey { get; set; }

        internal ChileStateEnum ChileState { get; set; }
    }
}