namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IProductAttributeRangeReturn
    {
        string AttributeName { get; set; }

        double MinValue { get; set; }

        double MaxValue { get; set; }

        string AttributeNameKey { get; set; }
    }
}