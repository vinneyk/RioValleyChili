// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ProductionScheduleProjectors
    {
        internal static Expression<Func<ProductionSchedule, ProductionScheduleKeyReturn>> SelectKey()
        {
            return p => new ProductionScheduleKeyReturn
                {
                    ProductionScheduleKey_ProductionDate = p.ProductionDate,
                    LocationKey_Id = p.ProductionLineLocationId
                };
        }

        internal static Expression<Func<ProductionSchedule, ProductionScheduleSummaryReturn>> SelectSummary()
        {
            var key = SelectKey();
            var location = LocationProjectors.SelectLocation();

            return Projector<ProductionSchedule>.To(p => new ProductionScheduleSummaryReturn
                {
                    ProductionScheduleKeyReturn = key.Invoke(p),
                    ProductionDate = p.ProductionDate,
                    ProductionLine = location.Invoke(p.ProductionLineLocation)
                });
        }

        internal static IEnumerable<Expression<Func<ProductionSchedule, ProductionScheduleDetailReturn>>> SelectDetail()
        {
            var key = SelectKey();
            var location = LocationProjectors.SelectLocation();
            var itemSelect = ProductionScheduleItemProjectors.Select();

            return new Projectors<ProductionSchedule, ProductionScheduleDetailReturn>
                {
                    p => new ProductionScheduleDetailReturn
                        {
                            ProductionScheduleKeyReturn = key.Invoke(p),
                            ProductionDate = p.ProductionDate,
                            ProductionLine = location.Invoke(p.ProductionLineLocation),
                        },
                    { itemSelect, p => i => new ProductionScheduleDetailReturn
                        {
                            ScheduledItems = i.ScheduledItems.OrderBy(n => n.Index).Select(n => p.Invoke(n))
                        }
                    }
                };
        }

        internal static IEnumerable<Expression<Func<ProductionSchedule, ProductionScheduleReportReturn>>> SelectReport()
        {
            var batchLotKey = LotProjectors.SelectLotKey<ProductionBatch>();
            var chileProduct = ProductProjectors.SelectChileProductSummary();
            var packScheduleKey = PackScheduleProjectors.SelectKey();

            return new Projectors<ProductionSchedule, ProductionScheduleReportReturn>
            {
                p => new ProductionScheduleReportReturn
                    {
                        Timestamp = p.TimeStamp,
                        ProductionDate = p.ProductionDate,
                        ProductionLocation = p.ProductionLineLocation.Description,

                        ScheduledItems = p.ScheduledItems
                            .Where(i => i.PackSchedule.ProductionBatches.Any(b => !b.ProductionHasBeenCompleted))
                            .Select(i => new ProductionScheduleItemReportReturn
                                {
                                    FlushBefore = i.FlushBefore,
                                    FlushBeforeInstructions = i.FlushBeforeInstructions,

                                    PackScheduleKeyReturn = packScheduleKey.Invoke(i.PackSchedule),
                                    CustomerName = new[] {i.PackSchedule.Customer}.Where(c => c != null).Select(c => c.Company.Name).FirstOrDefault(),
                                    WorkType = i.PackSchedule.WorkType.Description,
                                    
                                    Instructions = i.PackSchedule.SummaryOfWork,
                                    ProductionDeadline = i.PackSchedule.ProductionDeadline,
                                    OrderNumber = i.PackSchedule.OrderNumber,

                                    PackagingProduct = i.PackSchedule.PackagingProduct.Product.Name,

                                    FlushAfter = i.FlushAfter,
                                    FlushAfterInstructions = i.FlushAfterInstructions,

                                    ChileProductReturn = chileProduct.Invoke(i.PackSchedule.ChileProduct)
                                })
                    },
                p => new ProductionScheduleReportReturn
                    {
                        ScheduledItems = p.ScheduledItems
                            .Where(i => i.PackSchedule.ProductionBatches.Any(b => !b.ProductionHasBeenCompleted))
                            .Select(i => new ProductionScheduleItemReportReturn
                                {
                                    ProductionBatches = i.PackSchedule.ProductionBatches.Where(b => !b.ProductionHasBeenCompleted)
                                        .OrderBy(b => b.LotTypeId)
                                        .ThenBy(b => b.LotDateCreated)
                                        .ThenBy(b => b.LotDateSequence)
                                        .Select(b => new ProductionScheduleBatchReturn
                                            {
                                                LotKeyReturn = batchLotKey.Invoke(b)
                                            }),

                                    Granularity = i.PackSchedule.ChileProduct.Mesh,
                                    Scan = i.PackSchedule.ProductionBatches.Where(b => !b.ProductionHasBeenCompleted).Average(b => (double?)b.TargetParameters.BatchTargetScan)
                                })
                    },
            };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup