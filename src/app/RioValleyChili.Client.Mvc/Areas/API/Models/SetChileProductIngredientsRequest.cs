using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class SetChileProductIngredientsRequest
    {
        public IEnumerable<SetChileProductIngredientRequest> Ingredients { get; set; }
    }
}