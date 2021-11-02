using System.Collections.Generic;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Models
{
    public class ChileProductWithIngredients
    {
        public ChileProduct ChileProduct { get; set; }

        public IEnumerable<ChileProductIngredientWithAdditiveType> IngredientsWithAdditiveTypes { get; set; }
    }
}