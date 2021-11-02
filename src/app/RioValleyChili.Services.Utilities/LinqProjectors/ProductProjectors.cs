// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ProductProjectors
    {
        internal static Expression<Func<Product, ProductKeyReturn>> SelectProductKey()
        {
            return p => new ProductKeyReturn
            {
                ProductKey_ProductId = p.Id
            };
        }

        internal static Expression<Func<Product, ProductKeyNameReturn>> SelectProductKeyName()
        {
            var productKey = SelectProductKey();

            return p => new ProductKeyNameReturn
                {
                    ProductKeyReturn = productKey.Invoke(p),
                    ProductName = p.Name
                };
        }

        internal static Expression<Func<Product, ProductBaseReturn>> SelectProduct()
        {
            return SelectProductKeyName().Merge(p => new ProductBaseReturn
                {
                    ProductCode = p.ProductCode,
                    IsActive = p.IsActive,
                    ProductType = p.ProductType
                });
        }

        internal static Expression<Func<ChileType, ChileTypeKeyReturn>> SelectChileTypeKey()
        {
            return c => new ChileTypeKeyReturn
            {
                ChileTypeKey_ChileTypeId = c.Id
            };
        }

        internal static Expression<Func<AdditiveType, AdditiveTypeKeyReturn>> SelectAdditiveTypeKey()
        {
            return a => new AdditiveTypeKeyReturn
            {
                AdditiveTypeKey_AdditiveTypeId = a.Id
            };
        }

        internal static Expression<Func<AdditiveProduct, AdditiveProductReturn>> SelectAdditiveProduct()
        {
            var product = SelectProduct();
            Expression<Func<AdditiveProduct, AdditiveProductReturn>> additiveProduct = a => new AdditiveProductReturn
                {
                    AdditiveTypeId = a.AdditiveTypeId,
                    AdditiveTypeDescription = a.AdditiveType.Description,
                };
            return product.Merge(additiveProduct, a => a.Product);
        }

        internal static Expression<Func<PackagingProduct, PackagingProductReturn>> SelectPackagingProduct()
        {
            var product = SelectProduct();
            Expression<Func<PackagingProduct, PackagingProductReturn>> packagingProduct = p => new PackagingProductReturn
                            {
                                PackagingProductId = p.Id,
                                Weight = p.Weight,
                                PackagingWeight = p.PackagingWeight,
                                PalletWeight = p.PalletWeight
                            };
            return product.Merge(packagingProduct, p => p.Product);
        }
        
        internal static Expression<Func<ChileProduct, ChileProductReturn>> SelectChileProductSummary()
        {
            var product = SelectProduct();
            var chileTypeKey = SelectChileTypeKey();

            return product.Merge(Projector<ChileProduct>.To(c => new ChileProductReturn
                {
                    ChileTypeKeyReturn = chileTypeKey.Invoke(c.ChileType),
                    ChileTypeDescription = c.ChileType.Description,
                    ChileState = c.ChileState,
                }), c => c.Product);
        }

        internal static Expression<Func<ChileProduct, ChileProductReturn>> SelectChileProductDetail()
        {
            var ingredient = SelectChileProductIngredient();
            var attributeRange = SelectChileProductAttributeRange();

            return SelectChileProductSummary().Merge(c => new ChileProductReturn
                {
                    Mesh = c.Mesh,
                    IngredientsDescription = c.IngredientsDescription,
                    ProductIngredients = c.Ingredients.Select(i => ingredient.Invoke(i)),
                    AttributeRanges = c.ProductAttributeRanges.Select(r => attributeRange.Invoke(r))
                });
        }

        internal static Expression<Func<ChileProductIngredient, ProductIngredientReturn>> SelectChileProductIngredient()
        {
            var additiveTypeKey = SelectAdditiveTypeKey();
            return i => new ProductIngredientReturn
                {
                    AdditiveTypeKeyReturn = additiveTypeKey.Invoke(i.AdditiveType),
                    AdditiveTypeName = i.AdditiveType.Description,
                    Percent = i.Percentage
                };
        }

        internal static Expression<Func<ChileProductAttributeRange, ChileProductAttributeRangeReturn>> SelectChileProductAttributeRange()
        {
            return r => new ChileProductAttributeRangeReturn
                {
                    AttributeNameKey = r.AttributeName.ShortName,
                    AttributeName = r.AttributeName.Name,
                    MinValue = r.RangeMin,
                    MaxValue = r.RangeMax
                };
        }

        internal static Expression<Func<ChileProduct, ChileProductWithIngredients>> SelectChileProductWithIngredients()
        {
            return p => new ChileProductWithIngredients
                {
                    ChileProduct = p,
                    IngredientsWithAdditiveTypes = p.Ingredients.Select(i => new ChileProductIngredientWithAdditiveType
                        {
                            ChileProductIngredient = i,
                            AdditiveType = i.AdditiveType
                        })
                };
        }

        internal static Expression<Func<ChileProduct, LabReportChileProduct>> SelectLabReportChileProduct()
        {
            var attributeRange = SelectChileProductAttributeRange();

            Expression<Func<ChileProduct, LabReportChileProduct>> chileProduct = c => new LabReportChileProduct
                {
                    AttributeRangeReturns = c.ProductAttributeRanges.Select(r => attributeRange.Invoke(r))
                };

            return SelectProduct().Merge(chileProduct, c => c.Product);
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup