using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetChileProductIngredientsParameters : ISetChileProductIngredientsParameters
    {
        public string UserToken { get; set; }
        public string ChileProductKey { get; set; }
        public IEnumerable<ISetChileProductIngredientParameters> Ingredients { get; set; }
    }
}