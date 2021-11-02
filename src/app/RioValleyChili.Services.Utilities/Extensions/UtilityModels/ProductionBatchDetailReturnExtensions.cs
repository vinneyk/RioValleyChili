using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.Extensions.UtilityModels
{
    internal static class ProductionBatchDetailReturnExtensions
    {
        internal static IEnumerable<ProductionBatchPackagingMaterialSummary> ToPackagingMaterialSummaries(this ProductionBatchDetailReturn productionBatch)
        {
            var productKeyProjector = LinqProjectors.ProductProjectors.SelectProductKey();
            return productionBatch.PickedPackagingItems.Select(i => new ProductionBatchPackagingMaterialSummary
                {
                    PackagingProductKeyReturn = productKeyProjector.Invoke(i.Product),
                    PackagingDescription = i.Product.Name,
                    QuantityPicked = i.PickedInventoryItem.Quantity,
                    TotalQuantityNeeded = (int) (productionBatch.PackagingProduct.Weight <= 0 ? 0 :
                        productionBatch.BatchTargetWeight / productionBatch.PackagingProduct.Weight)
                });
        }

        internal static ProductionBatchMaterialsSummary ToFinishedGoodsMaterialSummary(this ProductionBatchDetailReturn productionBatch)
        {
            var totalWeightPicked = productionBatch.PickedChileItems
                                                         .Where(c => c.ChileLot.LotTypeId == LotTypeEnum.FinishedGood.ToInt())
                                                         .Sum(c => c.PickedInventoryItem.Quantity * c.Packaging.Weight);

            return new ProductionBatchMaterialsSummary
                {
                    ProductType = ProductTypeEnum.Chile,
                    LotType = LotTypeEnum.FinishedGood,
                    IngredientName = "Finished Goods",
                    TargetPercentage = 0,
                    TargetWeight = 0,
                    WeightPicked = (int) totalWeightPicked
                };
        }

        internal static ProductionBatchMaterialsSummary ToWipMaterialsSummary(this ProductionBatchDetailReturn productionBatch, List<ProductionBatchMaterialsSummary> chileProductAdditiveIngredientSummaries)
        {
            var additiveIngredientPercentage = productionBatch.ChileProductWithIngredients.IngredientsWithAdditiveTypes.Sum(i => i.ChileProductIngredient.Percentage);
            var wipWeightPicked = productionBatch.PickedChileItems
                                                       .Where(c => c.ChileLot.LotTypeId == (int) LotTypeEnum.WIP)
                                                       .Sum(c => c.PickedInventoryItem.Quantity * c.Packaging.Weight);

            var wipTargetPercentage = Math.Max(0.0, 1.0 - additiveIngredientPercentage);
            var wipTargetWeight = productionBatch.BatchTargetWeight * wipTargetPercentage;

            return new ProductionBatchMaterialsSummary
                {
                    ProductType = ProductTypeEnum.Chile,
                    LotType = LotTypeEnum.WIP,
                    IngredientName = "WIP",
                    TargetPercentage = wipTargetPercentage,
                    TargetWeight = wipTargetWeight,
                    WeightPicked = wipWeightPicked
                };
        }

        internal static IEnumerable<ProductionBatchMaterialsSummary> ToChileProductAdditiveIngredientSummaries(this ProductionBatchDetailReturn productionBatch)
        {
            return productionBatch.ChileProductWithIngredients.IngredientsWithAdditiveTypes == null ? new List<ProductionBatchMaterialsSummary>() :
                (from ingredient in productionBatch.ToChileProductIngredientsWithUndefinedPickedAdditives()
                 let totalWeightPicked = productionBatch.PickedAdditiveItems.Where(a => a.AdditiveType.Id == ingredient.AdditiveTypeId)
                    .Sum(a => a.PickedInventoryItem.Quantity * a.Packaging.Weight)
                 select new ProductionBatchMaterialsSummary
                     {
                         ProductType = ProductTypeEnum.Additive,
                         LotType = LotTypeEnum.Additive,
                         ChileProductIngredientKey_AdditiveTypeId = ingredient.AdditiveTypeId,
                         IngredientName = ingredient.AdditiveType.Description,
                         TargetPercentage = ingredient.Percentage,
                         TargetWeight = (int)(productionBatch.BatchTargetWeight * ingredient.Percentage),
                         WeightPicked = (int)totalWeightPicked
                     });
        }

        private static IEnumerable<ChileProductIngredient> ToChileProductIngredientsWithUndefinedPickedAdditives(this ProductionBatchDetailReturn productionBatch)
        {
            var undefinedIngredients = productionBatch.PickedAdditiveItems
                                                            .Where(a => productionBatch.ChileProductWithIngredients.IngredientsWithAdditiveTypes.All(i => i.AdditiveType.Id != a.AdditiveProduct.AdditiveTypeId))
                                                            .Select(i => new ChileProductIngredient
                                                                {
                                                                    AdditiveTypeId = i.AdditiveType.Id,
                                                                    ChileProduct = productionBatch.ChileProductWithIngredients.ChileProduct,
                                                                    ChileProductId = productionBatch.ChileProductWithIngredients.ChileProduct.Id,
                                                                    AdditiveType = i.AdditiveType,
                                                                    Percentage = 0
                                                                });
            return productionBatch.ChileProductWithIngredients.IngredientsWithAdditiveTypes.Select(i => i.ChileProductIngredient).Concat(undefinedIngredients);
        }
    }
}