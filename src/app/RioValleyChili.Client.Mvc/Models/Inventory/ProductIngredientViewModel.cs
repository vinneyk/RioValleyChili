using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Client.Mvc.Models.Inventory
{
    public class ProductIngredientViewModel : IProductIngredientReturn
    {
        public double Percent { get; set; }
        public string AdditiveTypeKey { get; set; }
        public string AdditiveTypeName { get; set; }
    }
}