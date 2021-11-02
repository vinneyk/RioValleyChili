namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface ISetChileProductIngredientParameters
    {
        string AdditiveTypeKey { get; }
        double Percentage { get; }
    }
}