// ReSharper disable RedundantExtendsListEntry
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateProductDto : ICreateProductParameters, ICreateAdditiveProductParameters, ICreateChileProductParameters, ICreatePackagingProductParameters
    {
        public string UserToken { get; set; }
        [Required]
        public string ProductCode { get; set; }
        [Required]
        public string ProductName { get; set; }
        public ProductTypeEnum ProductType { get; set; }

        public ChileProductParameters ChileProductParameters { get; set; }
        public AdditiveProductParameters AdditiveProductParameters { get; set; }
        public PackagingProductParameters PackagingProductParameters { get; set; }

        #region Explicit Interface Implementations

        #region Additive products

        string ICreateAdditiveProductParameters.AdditiveTypeKey { get { return AdditiveProductParameters.AdditiveTypeKey; } }

        #endregion

        #region Chile products

        ChileStateEnum ICreateChileProductParameters.ChileState { get { return ChileProductParameters.ChileState; } }
        string ICreateChileProductParameters.ChileTypeKey { get { return ChileProductParameters.ChileTypeKey; } }
        double? ICreateChileProductParameters.Mesh { get { return ChileProductParameters.Mesh; } }
        string ICreateChileProductParameters.IngredientsDescription { get { return ChileProductParameters.IngredientsDescription; } }

        IEnumerable<ISetAttributeRangeParameters> ICreateChileProductParameters.AttributeRanges
        {
            get
            {
                return ChileProductParameters.AttributeRanges.Select(r => new SetAttributeRangeParameters
                    {
                        AttributeNameKey = r.AttributeNameKey,
                        RangeMin = r.RangeMin,
                        RangeMax = r.RangeMax
                    });
            }
        }

        IEnumerable<ISetChileProductIngredientParameters> ICreateChileProductParameters.Ingredients
        {
            get
            {
                return ChileProductParameters.Ingredients.Select(i => new SetChileProductIngredientParameters
                    {
                        AdditiveTypeKey = i.AdditiveTypeKey,
                        Percentage = i.Percentage
                    });
            }
        }

        #endregion

        #region Packaging products

        double ICreatePackagingProductParameters.Weight { get { return PackagingProductParameters.Weight; } }
        double ICreatePackagingProductParameters.PackagingWeight { get { return PackagingProductParameters.PackagingWeight; } }
        double ICreatePackagingProductParameters.PalletWeight { get { return PackagingProductParameters.PalletWeight; } }

        #endregion

        #endregion
    }

    public class ChileProductParameters : IChileProductParameters
    {
        public string ChileTypeKey { get; set; }
        public double? Mesh { get; set; }
        public string IngredientsDescription { get; set; }
        public ChileStateEnum ChileState { get; set; }

        public IEnumerable<AttributeRangeRequest> AttributeRanges { get; set; }
        public IEnumerable<SetChileProductIngredientRequest> Ingredients { get; set; }
    }

    public class AdditiveProductParameters : IAdditiveProductParameters
    {
        public string AdditiveTypeKey { get; set; }
    }

    public class PackagingProductParameters : IPackagingProductParameters
    {
        public double Weight { get; set; }
        public double PackagingWeight { get; set; }
        public double PalletWeight { get; set; }
    }
}
// ReSharper restore RedundantExtendsListEntry