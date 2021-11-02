// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ProductionBatchProjectors
    {
        internal static Expression<Func<ProductionBatch, ProductionBatchSummaryReturn>> SelectSummary()
        {
            var lotKey = LotProjectors.SelectLotKey<ProductionBatch>();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();

            return b => new ProductionBatchSummaryReturn
                {
                    PackagingProduct = packagingProduct.Invoke(b.PackSchedule.PackagingProduct),
                    OutputLotKeyReturn = lotKey.Invoke(b),
                    HasProductionBeenCompleted = b.ProductionHasBeenCompleted,
                    BatchTargetWeight = b.TargetParameters.BatchTargetWeight,
                    BatchTargetAsta = b.TargetParameters.BatchTargetAsta,
                    BatchTargetScan = b.TargetParameters.BatchTargetScan,
                    BatchTargetScoville = b.TargetParameters.BatchTargetScoville,
                    Notes = b.Production.ResultingChileLot.Lot.Notes
                };
        }

        #region SplitSelectDetail

        internal static IEnumerable<Expression<Func<ProductionBatch, ProductionBatchDetailReturn>>> SplitSelectDetail(Data.Interfaces.UnitsOfWork.IProductionUnitOfWork productionUnitOfWork, DateTime currentDate)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }

            var packScheduleKey = PackScheduleProjectors.SelectKey();
            var productKey = ProductProjectors.SelectProductKey();
            var chileProduct = ProductProjectors.SelectChileProductWithIngredients();
            var workType = WorkTypeProjectors.Select();
            var notebook = NotebookProjectors.Select();
            var pickedChile = PickedInventoryProjectors.SelectPickedChileInventoryItem(productionUnitOfWork);
            var pickedPackaging = PickedInventoryProjectors.SelectPickedPackagingInventoryItem(productionUnitOfWork);
            var pickedAdditve = PickedInventoryProjectors.SelectPickedAdditiveInventoryItem(productionUnitOfWork);
            var pickedItemProjectors = PickedInventoryItemProjectors.SplitSelect(productionUnitOfWork, currentDate);

            return new[]
                {
                    SelectSummary().Merge(b => new ProductionBatchDetailReturn
                        {
                            HasProductionBeenCompleted = b.ProductionHasBeenCompleted,
                        }).ExpandAll(),
                    Projector<ProductionBatch>.To(b => new ProductionBatchDetailReturn
                        {
                            PackScheduleKeyReturn = packScheduleKey.Invoke(b.PackSchedule),
                            ChileProductKeyReturn = productKey.Invoke(b.Production.ResultingChileLot.ChileProduct.Product),
                            ChileProductName = b.Production.ResultingChileLot.ChileProduct.Product.Name,
                            ChileProductWithIngredients = chileProduct.Invoke(b.Production.ResultingChileLot.ChileProduct)
                        }),
                    Projector<ProductionBatch>.To(b => new ProductionBatchDetailReturn
                        {
                            WorkType = workType.Invoke(b.PackSchedule.WorkType),
                            InstructionsNotebook = notebook.Invoke(b.InstructionNotebook)
                        }),
                    Projector<ProductionBatch>.To(b => new ProductionBatchDetailReturn
                        {
                            PickedChileItems = pickedChile.Invoke(b.Production.PickedInventory),
                            PickedPackagingItems = pickedPackaging.Invoke(b.Production.PickedInventory),
                            PickedAdditiveItems = pickedAdditve.Invoke(b.Production.PickedInventory)
                        })
                }.ToAppendedList(pickedItemProjectors.Select(p => Projector<ProductionBatch>.To(b => new ProductionBatchDetailReturn
                    {
                        PickedInventoryItems = b.Production.PickedInventory.Items.Select(i => p.Invoke(i))
                    })));
        }

        #endregion

        internal static Expression<Func<ProductionBatch, ScheduledProductionBatchReturn>> SelectScheduledProductionBatch()
        {
            var lotKey = LotProjectors.SelectLotKey<ProductionBatch>();

            return b => new ScheduledProductionBatchReturn
                {
                    OutputLotKeyReturn = lotKey.Invoke(b)
                };
        }

        internal static IEnumerable<Expression<Func<ProductionBatch, ProductionPacketBatchReturn>>> SplitSelectProductionPacket(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var lotKey = LotProjectors.SelectLotKey<ProductionBatch>();
            var pickedItem = PickedInventoryItemProjectors.SplitSelect(inventoryUnitOfWork, currentDate);
            var instructions = NotebookProjectors.Select();

            return pickedItem.Select(p => Projector<ProductionBatch>.To(b => new ProductionPacketBatchReturn
                {
                    PickedItems = b.Production.PickedInventory.Items.Select(i => p.Invoke(i))
                }))
                .ToAppendedList(b => new ProductionPacketBatchReturn
                    {
                        LotKeyReturn = lotKey.Invoke(b),
                        Notes = b.Production.ResultingChileLot.Lot.Notes,
                        TargetParameters = new ProductionBatchTargetParametersReturn
                            {
                                BatchTargetWeight = b.TargetParameters.BatchTargetWeight,
                                BatchTargetAsta = b.TargetParameters.BatchTargetAsta,
                                BatchTargetScoville = b.TargetParameters.BatchTargetScoville,
                                BatchTargetScan = b.TargetParameters.BatchTargetScan
                            }
                    },
                    b => new ProductionPacketBatchReturn
                        {
                            Instructions = instructions.Invoke(b.InstructionNotebook)
                        });
        }

        internal static IEnumerable<Expression<Func<ProductionBatch, ProductionPacketReturn>>> SplitSelectProductionPacketFromBatch(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, ILotKey batchKey)
        {
            return PackScheduleProjectors.SplitSelectProductionPacket(inventoryUnitOfWork, currentDate, batchKey)
                .Select(b => Projector<ProductionBatch>.To(a => b.Invoke(a.PackSchedule)));
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup