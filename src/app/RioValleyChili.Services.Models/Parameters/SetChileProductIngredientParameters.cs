using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetChileProductIngredientParameters : ISetChileProductIngredientParameters
    {
        public string AdditiveTypeKey { get; set; }
        public double Percentage { get; set; }
    }
}