using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetChileProductIngredientsParameters : ISetChileProductIngredientsParameters
    {
        public string UserToken { get; set; }
        public string ChileProductKey { get; set; }

        public IEnumerable<SetChileProductIngredientParameters> Ingredients { get; set; }

        IEnumerable<ISetChileProductIngredientParameters> ISetChileProductIngredientsParameters.Ingredients { get { return Ingredients; } }
    }
}