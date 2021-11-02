// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotProductionResultsProjectors
    {
        internal static IEnumerable<Expression<Func<LotProductionResults, ProductionResultReturn>>> SplitSelectSummary(IQueryable<ProductionBatch> productionBatches)
        {
            return SplitSelectReturn().ToAppendedList(SelectBatchResultsSummary(productionBatches));
        }

        internal static IEnumerable<Expression<Func<LotProductionResults, ProductionResultReturn>>> SplitSelectDetail(IQueryable<ProductionBatch> productionBatches)
        {
            var resultItemProjectors = LotProductionResultItemProjectors.Select();
            return SplitSelectReturn()
                .ToAppendedList(SelectBatchResultsSummary(productionBatches).Merge(p => new ProductionResultReturn
                    {
                        ResultItems = p.ResultItems.Select(i => resultItemProjectors.Invoke(i)),
                    }).ExpandAll());
        }

        internal static Expression<Func<LotProductionResults, ProductionRecapLot>> SelectProductionRecap(IQueryable<ProductionBatch> productionBatches)
        {
            var product = ProductProjectors.SelectChileProductSummary();
            var location = LocationProjectors.SelectLocation();
            var lotKey = LotProjectors.SelectLotKey<ChileLotProduction>();
            var batchPredicate = ProductionBatchPredicates.ByLotKeyEntity<LotProductionResults>();
            var batchSelect = PackScheduleProjectors.SelectProductionRecap();

            return Projector<LotProductionResults>.To(p => new ProductionRecapLot
                {
                    ProductionType = p.Production.ProductionType,
                    LotProductionStatus = p.Production.ResultingChileLot.Lot.ProductionStatus,
                    LotQualityStatus = p.Production.ResultingChileLot.Lot.QualityStatus,
                    OutOfSpec = p.Production.ResultingChileLot.Lot.ProductSpecOutOfRange,
                    ProductionBegin = p.ProductionBegin,
                    ProductionEnd = p.ProductionEnd,
                    ProductionLocation = location.Invoke(p.ProductionLineLocation),

                    ChileProduct = product.Invoke(p.Production.ResultingChileLot.ChileProduct),
                    LotKey = lotKey.Invoke(p.Production),
                    TotalInputWeight = p.Production.PickedInventory.Items.Any() ? p.Production.PickedInventory.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0.0,
                    TotalOutputWeight = p.ResultItems.Any() ? p.ResultItems.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0.0,
                    Shift = p.ShiftKey,

                    ProductionBatch = productionBatches.Where(b => batchPredicate.Invoke(p, b)).Select(b => batchSelect.Invoke(b.PackSchedule)).FirstOrDefault(),
                    UnresolvedDefects = p.Production.ResultingChileLot.Lot.LotDefects.Where(d => d.Resolution == null).Select(d => d.Description)
                });
        }

        internal static Expression<Func<LotProductionResults, ProductionResultBaseReturn>> SelectBase()
        {
            var location = LocationProjectors.SelectLocation();

            return r => new ProductionResultBaseReturn
                {
                    ProductionEndDate = r.ProductionEnd,
                    ProductionShiftKey = r.ShiftKey,
                    ProductionLocation = location.Invoke(r.ProductionLineLocation)
                };
        }

        internal static Expression<Func<LotProductionResults, ProductionAdditiveInputs>> SelectAdditiveInputs(ILotUnitOfWork lotUnitOfWork)
        {
            var lotKey = LotProjectors.SelectLotKey<LotProductionResults>();
            var lotProduct = ProductProjectors.SelectProduct();
            var additiveLots = LotProjectors.SelectAdditiveLot(lotUnitOfWork);
            var additiveProduct = ProductProjectors.SelectAdditiveProduct();
            var additiveLotKey = LotProjectors.SelectLotKey<AdditiveLot>();

            return r => new ProductionAdditiveInputs
                {
                    ProductionDate = r.ProductionBegin,
                    LotKeyReturn = lotKey.Invoke(r),
                    LotProduct = lotProduct.Invoke(r.Production.ResultingChileLot.ChileProduct.Product),
                    PickedAdditiveItems = r.Production.PickedInventory.Items.Select(i => new
                        {
                            Item = i,
                            AdditiveLot = additiveLots.Invoke(i.Lot).FirstOrDefault()
                        })
                        .Where(i => i.AdditiveLot != null)
                        .Select(i => new ProductionPickedAdditive
                            {
                                LotKeyReturn = additiveLotKey.Invoke(i.AdditiveLot),
                                TotalPoundsPicked = i.Item.Quantity * i.Item.PackagingProduct.Weight,
                                AdditiveProduct = additiveProduct.Invoke(i.AdditiveLot.AdditiveProduct),
                                UserResultEntered = r.Employee.UserName
                            })
                };
        }

        private static Expression<Func<LotProductionResults, ProductionResultReturn>> SelectBatchResultsSummary(IQueryable<ProductionBatch> productionBatches)
        {
            return r => new ProductionResultReturn
                {
                    TargetBatchWeight = productionBatches.Where(b => b.LotTypeId == r.LotTypeId && b.LotDateCreated == r.LotDateCreated && b.LotDateSequence == r.LotDateSequence)
                        .Select(b => b.TargetParameters.BatchTargetWeight).FirstOrDefault(),

                    //todo: Update interface/implementations of following properties? -RI 7/1/13
                    BatchStatus = productionBatches.Where(b => b.LotTypeId == r.LotTypeId && b.LotDateCreated == r.LotDateCreated && b.LotDateSequence == r.LotDateSequence)
                        .Select(b => b.ProductionHasBeenCompleted ? "Completed" : "NotCompleted").FirstOrDefault(),
                    WorkType = productionBatches.Where(b => b.LotTypeId == r.LotTypeId && b.LotDateCreated == r.LotDateCreated && b.LotDateSequence == r.LotDateSequence)
                        .Select(b => b.PackSchedule.WorkType.Description).FirstOrDefault(),
                };
        }

        private static IEnumerable<Expression<Func<LotProductionResults, ProductionResultReturn>>> SplitSelectReturn()
        {
            var productionLocationKey = LocationProjectors.SelectLocationKey();
            var lotKey = LotProjectors.SelectLotKey<LotProductionResults>();
            var productKey = ProductProjectors.SelectProductKey();

            return new[]
                {
                    SelectBase().Merge(r => new ProductionResultReturn
                        {
                            LotKeyReturn = lotKey.Invoke(r),
                            DateTimeEntered = r.DateTimeEntered,
                            ProductionStartDate = r.ProductionBegin
                        }).ExpandAll(),

                    Projector<LotProductionResults>.To(r => new ProductionResultReturn
                        {
                            User = r.Employee.UserName,
                            ProductionLocationKeyReturn = productionLocationKey.Invoke(r.ProductionLineLocation)
                        }),

                    Projector<LotProductionResults>.To(r => new ProductionResultReturn
                        {
                            ChileProductName = r.Production.ResultingChileLot.ChileProduct.Product.Name,
                            ChileProductKeyReturn = productKey.Invoke(r.Production.ResultingChileLot.ChileProduct.Product)
                        })
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup