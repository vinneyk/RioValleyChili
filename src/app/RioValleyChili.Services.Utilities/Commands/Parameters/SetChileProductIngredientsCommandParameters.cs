using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetChileProductIngredientsCommandParameters
    {
        internal ISetChileProductIngredientsParameters Parameters { get; set; }

        internal ChileProductKey ChileProductKey { get; set; }

        internal IEnumerable<SetChileProductIngredientCommandParameters> Ingredients { get; set; }
    }

    internal class SetChileProductIngredientCommandParameters
    {
        internal ISetChileProductIngredientParameters Parameters { get; set; }
        internal AdditiveTypeKey AdditiveTypeKey { get; set; }
    }
}