namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface ISetAttributeRangeParameters
    {
        string AttributeNameKey { get; }
        double RangeMin { get; }
        double RangeMax { get; }
    }
}