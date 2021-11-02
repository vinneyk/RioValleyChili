using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface ICreateProductParameters : IUserIdentifiable
    {
        string ProductCode { get; }
        string ProductName { get; }
    }

    public interface IUpdateProductParameters : IUserIdentifiable
    {
        string ProductKey { get; }
        string ProductCode { get; }
        string ProductName { get; }
        bool IsActive { get; }
    }

    public interface ICreateChileProductParameters : ICreateProductParameters
    {
        ChileStateEnum ChileState { get; }
        string ChileTypeKey { get; }
        double? Mesh { get; }
        string IngredientsDescription { get; }

        IEnumerable<ISetAttributeRangeParameters> AttributeRanges { get; }
        IEnumerable<ISetChileProductIngredientParameters> Ingredients { get; }
    }

    public interface IUpdateChileProductParameters : IUpdateProductParameters
    {
        string ChileTypeKey { get; }
        double? Mesh { get; }
        string IngredientsDescription { get; }

        IEnumerable<ISetAttributeRangeParameters> AttributeRanges { get; }
        IEnumerable<ISetChileProductIngredientParameters> Ingredients { get; }
    }

    public interface ICreateAdditiveProductParameters : ICreateProductParameters
    {
        string AdditiveTypeKey { get; }
    }

    public interface IUpdateAdditiveProductParameters : IUpdateProductParameters
    {
        string AdditiveTypeKey { get; }
    }

    public interface ICreatePackagingProductParameters : ICreateProductParameters
    {
        double Weight { get; }
        double PackagingWeight { get; }
        double PalletWeight { get; }
    }

    public interface IUpdatePackagingProductParameters : IUpdateProductParameters
    {
        double Weight { get; }
        double PackagingWeight { get; }
        double PalletWeight { get; }
    }
}