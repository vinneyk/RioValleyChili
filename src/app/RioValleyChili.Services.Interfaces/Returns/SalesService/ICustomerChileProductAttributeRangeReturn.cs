namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerChileProductAttributeRangeReturn
    {
        string CustomerChileProductAttributeRangeKey { get; }
        string AttributeShortName { get; }
        double RangeMin { get; }
        double RangeMax { get; }
    }
}