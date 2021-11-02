using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Client.Mvc.Models.Inventory
{
    public class ProductAttributeRangeReturnViewModel : IProductAttributeRangeReturn
    {
        public string AttributeNameKey { get; set; }
        public string AttributeName { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}