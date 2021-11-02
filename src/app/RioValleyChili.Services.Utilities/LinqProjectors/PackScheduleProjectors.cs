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
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class PackScheduleProjectors
    {
        internal static Expression<Func<PackSchedule, PackScheduleKeyReturn>> SelectKey()
        {
            return p => new PackScheduleKeyReturn
            {
                PackScheduleKey_DateCreated = p.DateCreated,
                PackScheduleKey_DateSequence = p.SequentialNumber
            };
        }

        internal static IEnumerable<Expression<Func<PackSchedule, PackScheduleSummaryReturn>>> SplitSelectSummary()
        {
            var productionLocationKey = LocationProjectors.SelectLocationKey();
            var workTypeKey = WorkTypeProjectors.SelectWorkTypeKey();
            var productKey = ProductProjectors.SelectProductKey();
            var company = CustomerProjectors.SelectCompanyHeader();

            return new[]
                {
                    SelectBaseParameters().Merge(p => new PackScheduleSummaryReturn
                        {
                            DateCreated = p.DateCreated,
                            ScheduledProductionDate = p.ScheduledProductionDate,
                            ProductionDeadline = p.ProductionDeadline,
                            OrderNumber = p.OrderNumber
                        }).ExpandAll(),
                    Projector<PackSchedule>.To(p => new PackScheduleSummaryReturn
                        {
                            WorkTypeKeyReturn = workTypeKey.Invoke(p.WorkType),
                            ChileProductKeyReturn = productKey.Invoke(p.ChileProduct.Product),
                            ChileProductName = p.ChileProduct.Product.Name,
                        }),
                    Projector<PackSchedule>.To(p => new PackScheduleSummaryReturn
                        {
                            ProductionLocationKeyReturn = productionLocationKey.Invoke(p.ProductionLineLocation),
                            Customer = new[] { p.Customer }.Where(c => c != null).Select(c => company.Invoke(c)).FirstOrDefault(),
                        })
                };
        }

        internal static IEnumerable<Expression<Func<PackSchedule, PackScheduleDetailReturn>>> SplitSelectDetail()
        {
            var productKey = ProductProjectors.SelectProductKey();
            var productionBatches = ProductionBatchProjectors.SelectSummary();

            return SplitSelectSummary().Select(ConvertSummaryToDetail)
                .ToListWithModifiedElement(0, e => e.Merge(p => new PackScheduleDetailReturn
                    {
                        SummaryOfWork = p.SummaryOfWork,
                    }).ExpandAll())
                .ToAppendedList(
                    Projector<PackSchedule>.To(p => new PackScheduleDetailReturn
                        {
                            PackagingProductKeyReturn = productKey.Invoke(p.PackagingProduct.Product),
                            PackagingProductName = p.PackagingProduct.Product.Name,
                            PackagingWeight = p.PackagingProduct.Weight,
                        }),
                    Projector<PackSchedule>.To(p => new PackScheduleDetailReturn
                        {
                            ProductionBatches = p.ProductionBatches.Select(b => productionBatches.Invoke(b))
                        }));
        }

        internal static IEnumerable<Expression<Func<PackSchedule, ProductionPacketReturn>>> SplitSelectProductionPacket(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, ILotKey batchKey = null)
        {
            var packScheduleKey = SelectKey();
            var companyHeader = CompanyProjectors.SelectHeader();
            var workType = WorkTypeProjectors.Select();
            var chileProduct = ProductProjectors.SelectProduct();
            var productKeyName = ProductProjectors.SelectProductKeyName();
            var productionBatchPredicate = batchKey != null ? ProductionBatchPredicates.ByLotKey(batchKey) : b => true;

            return ProductionBatchProjectors.SplitSelectProductionPacket(inventoryUnitOfWork, currentDate)
                .Select(b => Projector<PackSchedule>.To(p => new ProductionPacketReturn
                    {
                        Batches = p.ProductionBatches.Where(a => productionBatchPredicate.Invoke(a)).Select(a => b.Invoke(a))
                    }))
                .ToAppendedList(Projector<PackSchedule>.To(p => new ProductionPacketReturn
                    {
                        PackScheduleKeyReturn = packScheduleKey.Invoke(p),
                        PSNum = p.PSNum,
                        DateCreated = p.DateCreated,
                        SummaryOfWork = p.SummaryOfWork
                    }),
                    Projector<PackSchedule>.To(p => new ProductionPacketReturn
                    {
                        ChileProduct = chileProduct.Invoke(p.ChileProduct.Product),
                        PackagingProduct = productKeyName.Invoke(p.PackagingProduct.Product)
                    }),
                    Projector<PackSchedule>.To(p => new ProductionPacketReturn
                    {
                        ProductionLineDescription = p.ProductionLineLocation.Description,
                        WorkType = workType.Invoke(p.WorkType),
                        Customer = new[] { p.Customer }.Where(c => c != null).Select(c => companyHeader.Invoke(c.Company)).FirstOrDefault()
                    }));
        }

        internal static Expression<Func<PackSchedule, ProductionPacketReturn>> SelectProductionPacket(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, ILotKey batchKey = null)
        {
            return SplitSelectProductionPacket(inventoryUnitOfWork, currentDate, batchKey).Merge();
        }

        internal static IEnumerable<Expression<Func<PackSchedule, PackSchedulePickSheetReturn>>> SplitSelectPickSheet(IProductionUnitOfWork productionUnitOfWork, DateTime currentDate)
        {
            var product = ProductProjectors.SelectProductKeyName();
            var pickedItem = PickedInventoryItemProjectors.SplitSelectBatch(productionUnitOfWork, currentDate);

            return pickedItem.Select(p => Projector<PackSchedule>.To(a => new PackSchedulePickSheetReturn
                    {
                        PickedItems = a.ProductionBatches.SelectMany(b => b.Production.PickedInventory.Items.Select(i => p.Invoke(i)))
                    }))
                .ToAppendedList(SelectBase().Merge(p => new PackSchedulePickSheetReturn
                    {
                        SummaryOfWork = p.SummaryOfWork,
                        DateCreated = p.DateCreated,
                        ChileProduct = product.Invoke(p.ChileProduct.Product)
                    }).ExpandAll());
        }

        internal static Expression<Func<PackSchedule, PackScheduleBaseWithCustomerReturn>> SelectBaseWithCustomer()
        {
            var customer = SelectCustomerSpec();

            return SelectBaseParameters().Merge(p => new PackScheduleBaseWithCustomerReturn
                {
                    Customer = customer.Invoke(p)
                });
        }

        internal static Expression<Func<PackSchedule, CompanyHeaderReturn>> SelectCustomerHeader()
        {
            var customer = CustomerProjectors.SelectCompanyHeader();

            return p => new[] { p.Customer }.Where(c => c != null).Select(c => customer.Invoke(c)).FirstOrDefault();
        }

        internal static Expression<Func<PackSchedule, ProductionRecapBatch>> SelectProductionRecap()
        {
            var packScheduleKey = SelectKey();
            var workType = WorkTypeProjectors.Select();

            return p => new ProductionRecapBatch
                {
                    PSNum = p.PSNum,
                    PackScheduleKeyReturn = packScheduleKey.Invoke(p),
                    WorkType = workType.Invoke(p.WorkType)
                };
        }

        internal static IEnumerable<Expression<Func<PackSchedule, ScheduledPackScheduleReturn>>> SelectScheduled()
        {
            var key = SelectKey();
            var ab = SelectAverage(StaticAttributeNames.AB);
            var scoville = SelectAverage(StaticAttributeNames.Scoville);
            var scan = SelectAverage(StaticAttributeNames.Scan);
            var chileProduct = ProductProjectors.SelectChileProductSummary();

            return new Projectors<PackSchedule, ScheduledPackScheduleReturn>
                {
                    p => new ScheduledPackScheduleReturn
                        {
                            PackScheduleKeyReturn = key.Invoke(p),

                            ProductionDeadline = p.ProductionDeadline,
                            Instructions = p.SummaryOfWork,

                            ChileProduct = chileProduct.Invoke(p.ChileProduct)
                        },
                    p => new ScheduledPackScheduleReturn
                        {
                            AverageGranularity = p.ChileProduct.Mesh,
                            AverageAoverB = ab.Invoke(p),
                            AverageScoville = scoville.Invoke(p),
                            AverageScan = scan.Invoke(p),
                        }
                };
        }

        private static Expression<Func<PackSchedule, CustomerWithProductSpecReturn>> SelectCustomerSpec()
        {
            var customer = CustomerProjectors.SelectProductSpec();

            return p => new[] { p.Customer }.Where(c => c != null).Select(c => customer.Invoke(c, p.ChileProductId)).FirstOrDefault();
        }

        private static Expression<Func<PackSchedule, PackScheduleBaseReturn>> SelectBase()
        {
            var packScheduleKey = SelectKey();

            return p => new PackScheduleBaseReturn
                {
                    PackScheduleKeyReturn = packScheduleKey.Invoke(p),
                    PSNum = p.PSNum,
                    WorkType = p.WorkType.Description,
                    ProductionLineDescription = p.ProductionLineLocation.Description
                };
        }

        private static Expression<Func<PackSchedule, PackScheduleBaseParametersReturn>> SelectBaseParameters()
        {
            return SelectBase().Merge(Projector<PackSchedule>.To(p => new PackScheduleBaseParametersReturn
                {
                    TargetParameters = new ProductionBatchTargetParametersReturn
                        {
                            BatchTargetWeight = p.DefaultBatchTargetParameters.BatchTargetWeight,
                            BatchTargetAsta = p.DefaultBatchTargetParameters.BatchTargetAsta,
                            BatchTargetScoville = p.DefaultBatchTargetParameters.BatchTargetScoville,
                            BatchTargetScan = p.DefaultBatchTargetParameters.BatchTargetScan
                        }
                }));
        }

        private static Expression<Func<PackSchedule, PackScheduleDetailReturn>> ConvertSummaryToDetail(Expression<Func<PackSchedule, PackScheduleSummaryReturn>> summary)
        {
            return summary.Merge(p => new PackScheduleDetailReturn { }).ExpandAll();
        }

        internal static Expression<Func<PackSchedule, double>> SelectAverage(IAttributeNameKey attribute)
        {
            var average = LotAttributeProjectors.SelectAverage(attribute);
            return Projector<PackSchedule>.To(p => average.Invoke(p.ProductionBatches.SelectMany(b => b.Production.ResultingChileLot.Lot.Attributes)));
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup