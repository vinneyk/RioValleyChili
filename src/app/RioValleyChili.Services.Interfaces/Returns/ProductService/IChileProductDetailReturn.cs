using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IChileProductDetailReturn : IChileProductReturn
    {
        double? Mesh { get; }
        string IngredientsDescription { get; }
        IEnumerable<IProductAttributeRangeReturn> AttributeRanges { get; }
        IEnumerable<IProductIngredientReturn> ProductIngredients { get; }
    }
}