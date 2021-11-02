namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IProductIngredientReturn
    {
        double Percent { get; set; }

        string AdditiveTypeKey { get; }

        string AdditiveTypeName { get; set; }
    }
}