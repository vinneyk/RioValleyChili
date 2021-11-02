// ReSharper disable RedundantExtendsListEntry
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class UpdateProductDto : IUpdateProductParameters, IUpdateAdditiveProductParameters, IUpdateChileProductParameters, IUpdatePackagingProductParameters
    {
        internal string ProductKey { get; set; }

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string ProductName { get; set; }
        
        public bool IsActive { get; set; }

        public string UserToken { get; set; }
        public ProductTypeEnum ProductType { get; set; }

        public ChileProductParameters ChileProductParameters { get; set; }
        public AdditiveProductParameters AdditiveProductParameters { get; set; }
        public PackagingProductParameters PackagingProductParameters { get; set; }

        #region explicit interface definitions
        
        string IUpdateProductParameters.ProductKey {  get { return ProductKey; } }
        string IUpdateAdditiveProductParameters.AdditiveTypeKey { get { return AdditiveProductParameters.AdditiveTypeKey; } }
        string IUpdateChileProductParameters.ChileTypeKey { get { return ChileProductParameters.ChileTypeKey; } }
        double? IUpdateChileProductParameters.Mesh { get { return ChileProductParameters.Mesh; } }
        string IUpdateChileProductParameters.IngredientsDescription { get { return ChileProductParameters.IngredientsDescription; } }

        IEnumerable<ISetAttributeRangeParameters> IUpdateChileProductParameters.AttributeRanges
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

        IEnumerable<ISetChileProductIngredientParameters> IUpdateChileProductParameters.Ingredients
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

        double IUpdatePackagingProductParameters.Weight { get { return PackagingProductParameters.Weight; } }
        double IUpdatePackagingProductParameters.PackagingWeight { get { return PackagingProductParameters.PackagingWeight; } }
        double IUpdatePackagingProductParameters.PalletWeight { get { return PackagingProductParameters.PalletWeight; } }

        #endregion

    }
}
// ReSharper restore RedundantExtendsListEntry