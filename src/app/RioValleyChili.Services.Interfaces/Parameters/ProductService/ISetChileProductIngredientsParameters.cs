using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface ISetChileProductIngredientsParameters : IUserIdentifiable
    {
        string ChileProductKey { get; }

        IEnumerable<ISetChileProductIngredientParameters> Ingredients { get; }
    }
}