using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal abstract class CreateProductParametersBase<T> where T : ICreateProductParameters
    {
        internal T Parameters { get; set; }
    }

    internal abstract class UpdateProductParametersBase<T> where T : IUpdateProductParameters
    {
        internal T Parameters { get; set; }

        internal ProductKey ProductKey { get; set; }
    }

    internal class CreateAdditiveProductParameters : CreateProductParametersBase<ICreateAdditiveProductParameters>
    {
        internal AdditiveTypeKey AdditiveTypeKey { get; set; }
    }

    internal class UpdateAdditiveProductParameters : UpdateProductParametersBase<IUpdateAdditiveProductParameters>
    {
        internal AdditiveTypeKey AdditiveTypeKey { get; set; }
    }

    internal class CreateChileProductParameters : CreateProductParametersBase<ICreateChileProductParameters>
    {
        internal ChileTypeKey ChileTypeKey { get; set; }

        internal List<SetChileProductAttributeRangeParameters> AttributeRanges { get; set; }
        internal List<SetChileProductIngredientCommandParameters> Ingredients { get; set; }
    }

    internal class UpdateChileProductParameters : UpdateProductParametersBase<IUpdateChileProductParameters>
    {
        internal ChileTypeKey ChileTypeKey { get; set; }

        internal List<SetChileProductAttributeRangeParameters> AttributeRanges { get; set; }
        internal List<SetChileProductIngredientCommandParameters> Ingredients { get; set; }
    }

    internal class CreatePackagingProductParameters : CreateProductParametersBase<ICreatePackagingProductParameters>
    {
    }

    internal class UpdatePackagingProductParameters : UpdateProductParametersBase<IUpdatePackagingProductParameters>
    {
    }
}