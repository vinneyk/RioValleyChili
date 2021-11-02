using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class RemoveChileProductIngredientParameters
    {
        internal IRemoveChileProductIngredientParameters Parameters { get; set; }

        internal ChileProductIngredientKey ChileProductIngredientKey { get; set; }
    }
}