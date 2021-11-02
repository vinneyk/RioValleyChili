using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class AdditiveProductCommandParameters
    {
        internal IAdditiveProductParameters Parameters { get; set; }

        internal AdditiveTypeKey AdditiveTypeKey { get; set; }
    }
}