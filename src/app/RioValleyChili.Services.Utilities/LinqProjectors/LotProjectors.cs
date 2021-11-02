// ReSharper disable ConvertClosureToMethodGroup
// ReSharper disable RedundantEmptyObjectOrCollectionInitializer

using EF_Projectors;
using EF_Projectors.Extensions;
using RioValleyChili.Services.Utilities.LinqPredicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotProjectors
    {
        internal static Expression<Func<TEntity, LotKeyReturn>> SelectLotKey<TEntity>()
            where TEntity : LotKeyEntityBase
        {
            return l => new LotKeyReturn
                {
                    LotKey_DateCreated = l.LotDateCreated,
                    LotKey_DateSequence = l.LotDateSequence,
                    LotKey_LotTypeId = l.LotTypeId
                };
        }

        internal static Expression<Func<Lot, Product>> SelectProduct(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var chileLot = SelectChileLot(lotUnitOfWork);
            var additiveLot = SelectAdditiveLot(lotUnitOfWork);
            var packagingLot = SelectPackagingLot(lotUnitOfWork);

            return lot => chileLot.Invoke(lot).Select(c => c.ChileProduct.Product).Concat(additiveLot.Invoke(lot).Select(a => a.AdditiveProduct.Product)).Concat(packagingLot.Invoke(lot).Select(p => p.PackagingProduct.Product)).FirstOrDefault();
        }

        internal static Expression<Func<Lot, IQueryable<ChileLot>>> SelectChileLot(ILotUnitOfWork lotUnitOfWork)
        {
            var chileLots = lotUnitOfWork.ChileLotRepository.All();
            var lotPredicate = LotPredicates.ConstructLotKeyPredicate<Lot, ChileLot>();
            return lot => chileLots.Where(lotPredicate.Invoke(lot)).Take(1);
        }

        internal static Expression<Func<Lot, IQueryable<AdditiveLot>>> SelectAdditiveLot(ILotUnitOfWork lotUnitOfWork)
        {
            var additiveLots = lotUnitOfWork.AdditiveLotRepository.All();
            var lotPredicate = LotPredicates.ConstructLotKeyPredicate<Lot, AdditiveLot>();
            return lot => additiveLots.Where(lotPredicate.Invoke(lot)).Take(1);
        }

        internal static Expression<Func<Lot, IQueryable<PackagingLot>>> SelectPackagingLot(ILotUnitOfWork lotUnitOfWork)
        {
            var packagingLots = lotUnitOfWork.PackagingLotRepository.All();
            var lotPredicate = LotPredicates.ConstructLotKeyPredicate<Lot, PackagingLot>();
            return lot => packagingLots.Where(lotPredicate.Invoke(lot)).Take(1);
        }

        internal static Expression<Func<Lot, IQueryable<LotProductionResults>>> SelectChileLotResults(ILotUnitOfWork lotUnitOfWork)
        {
            var results = lotUnitOfWork.LotProductionResultsRepository.All();
            var lotPredicate = LotPredicates.ConstructLotKeyPredicate<Lot, LotProductionResults>();
            return lot => results.Where(lotPredicate.Invoke(lot)).Take(1);
        }

        internal static Expression<Func<Lot, InventoryProductReturn>> SelectDerivedProduct()
        {
            var selectChileProduct = SelectInventoryChileProductReturn();
            var selectAdditiveProduct = SelectInventoryAdditiveProductReturn();
            var selectPackagingProduct = SelectInventoryPackagingProductReturn();

            return Projector<Lot>.To(l =>
                new[] { l.ChileLot }.Where(c => c != null).Select(c => selectChileProduct.Invoke(c))
                    .Concat(new[] {l.AdditiveLot}.Where(a => a != null).Select(a => selectAdditiveProduct.Invoke(a)))
                    .Concat(new[] {l.PackagingLot}.Where(p => p != null).Select(p => selectPackagingProduct.Invoke(p)))
                    .FirstOrDefault());
        }

        #region SplitSelectLotSummary

        internal static IEnumerable<Expression<Func<Lot, LotSummaryReturn>>> SplitSelectLotSummary(ILotUnitOfWork lotUnitOfWork, DateTime currentDate)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var attributes = LotAttributeProjectors.Select();
            var lotDefects = LotDefectProjectors.SelectLotDefect();
            var product = SelectDerivedProduct();
            var loBac = SelectLoBac();
            var astaCalc = SelectAstaCalc(lotUnitOfWork, currentDate);

            var customer = PackScheduleProjectors.SelectCustomerHeader();
            var productionBatches = lotUnitOfWork.ProductionBatchRepository.All();
            var lotKeyFilter = LotPredicates.ConstructLotKeyPredicate<Lot, ProductionBatch>();

            return new[]
                {
                    SelectLotBase().Merge(l => new LotSummaryReturn
                        {
                            LotDateCreated = l.LotDateCreated,
                        }).ExpandAll(),

                    Projector<Lot>.To(l => new LotSummaryReturn
                        {
                            AstaCalcDate = currentDate,
                            Attributes = l.Attributes.Select(a => attributes.Invoke(a)),
                            Defects = l.LotDefects.OrderBy(d => d.DefectId).Select(d => lotDefects.Invoke(d))
                        }),

                    Projector<Lot>.To(l => new LotSummaryReturn
                        {
                            LoBac = loBac.Invoke(l),
                            LotProduct = product.Invoke(l),
                        }),

                    Projector<Lot>.To(l => new LotSummaryReturn
                        {
                            AstaCalc = astaCalc.Invoke(l),
                        }),

                    Projector<Lot>.To(l => new LotSummaryReturn
                        {
                            Customer = productionBatches
                                .Where(lotKeyFilter.Invoke(l))
                                .Select(b => customer.Invoke(b.PackSchedule)).FirstOrDefault()
                        })
                };
        }

        #endregion
        
        internal static IEnumerable<Expression<Func<Lot, LotQualitySummaryReturn>>> SplitSelectLotQualitySummary(ILotUnitOfWork lotUnitOfWork, DateTime currentDate)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var customerAllowance = LotCustomerAllowanceProjectors.Select();
            var customerOrderAllowance = LotCustomerOrderAllowanceProjectors.Select();
            var contractAllowance = LotContractAllowanceProjectors.Select();

            return SplitSelectLotSummary(lotUnitOfWork, currentDate)
                .Select(e => e.Merge(l => new LotQualitySummaryReturn { }).ExpandAll())
                .ToListWithModifiedElement(0, e => e.Merge(l => new LotQualitySummaryReturn
                    {
                        ProductSpecComplete = l.ProductSpecComplete,
                        ProductSpecOutOfRange = l.ProductSpecOutOfRange,
                    }).ExpandAll())
                .Concat(new[]
                    {
                        Projector<Lot>.To(l => new LotQualitySummaryReturn
                            {
                                CustomerAllowances = l.CustomerAllowances.Select(a => customerAllowance.Invoke(a)),
                                CustomerOrderAllowances = l.SalesOrderAllowances.Select(a => customerOrderAllowance.Invoke(a)),
                                ContractAllowances = l.ContractAllowances.Select(a => contractAllowance.Invoke(a))
                            })
                    });
        }

        internal static IEnumerable<Expression<Func<Lot, LotQualitySingleSummaryReturn>>> SplitSelectSingleLotSummary(ILotUnitOfWork lotUnitOfWork, DateTime currentDate)
        {
            var attributeNames = AttributeNameProjectors.SelectActiveAttributeNames(lotUnitOfWork);

            return SplitSelectLotQualitySummary(lotUnitOfWork, currentDate)
                .Select(p => Projector<Lot>.To(l => new LotQualitySingleSummaryReturn
                    {
                        LotSummary = p.Invoke(l)
                    }).ExpandAll())
                .ToAppendedList(Projector<Lot>.To(l => new LotQualitySingleSummaryReturn
                    {
                        AttributesByTypeReturn = new AttributesByTypeReturn
                            {
                                AttributeNamesAndTypes = attributeNames.Invoke()
                            }
                    }));
        }

        internal static Expression<Func<Lot, int?>> SelectAstaCalc(ILotUnitOfWork lotUnitOfWork, DateTime currentDate)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var calculateAsta = CalculateAsta(currentDate);
            var results = lotUnitOfWork.LotProductionResultsRepository.All();
            var lotKeyFilter = LotPredicates.ConstructLotKeyPredicate<Lot, LotProductionResults>();
            
            return Projector<Lot>.To(l => calculateAsta.Invoke
                (
                    l.Attributes.FirstOrDefault(a => a.AttributeShortName == GlobalKeyHelpers.AstaAttributeNameKey.AttributeNameKey_ShortName),
                    results.Where(lotKeyFilter.Invoke(l)).Select(r => (DateTime?)r.ProductionEnd).FirstOrDefault()
                ));
        }

        internal static Expression<Func<Lot, bool?>> SelectLoBac()
        {
            return l => new[] { l.ChileLot }.Where(c => c != null).Select(c => (bool?)c.AllAttributesAreLoBac).FirstOrDefault();
        }

        internal static Expression<Func<ChileLot, LabReportChileLotReturn>> SelectLabReportChileLot(IQueryable<ProductionBatch> productionBatches, IQueryable<LotProductionResults> lotProdcutionResuls, IQueryable<ChileLotProduction> chileLotProduction)
        {
            return SplitSelectLabReportChileLot(productionBatches, lotProdcutionResuls, chileLotProduction).Merge();
        }

        internal static IEnumerable<Expression<Func<ChileLot, LabReportChileLotReturn>>> SplitSelectLabReportChileLot(IQueryable<ProductionBatch> productionBatches, IQueryable<LotProductionResults> lotProdcutionResuls, IQueryable<ChileLotProduction> chileLotProduction)
        {
            var chileProductKey = ProductProjectors.SelectProductKey();
            var packSchedule = PackScheduleProjectors.SelectBaseWithCustomer();
            var productionResult = LotProductionResultsProjectors.SelectBase();
            var attribute = LotAttributeProjectors.Select<WeightedLotAttributeReturn>();
            var lotTotes = PickedInventoryItemProjectors.SelectPickedLot();
            var customerAllowance = LotCustomerAllowanceProjectors.Select();

            return new[]
                {
                    SelectLotBase().Merge(Projector<ChileLot>.To(c => new LabReportChileLotReturn
                        {
                            LoBac = c.AllAttributesAreLoBac
                        }), c => c.Lot).ExpandAll(),

                    Projector<ChileLot>.To(c => new LabReportChileLotReturn
                        {
                            UnresolvedDefects = c.Lot.LotDefects.Where(d => d.Resolution == null).Select(d => d.Description),
                            WeightedAttributes = c.Lot.Attributes.Select(a => attribute.Invoke(a)),
                            CustomerAllowances = c.Lot.CustomerAllowances.Select(a => customerAllowance.Invoke(a))
                        }),

                    Projector<ChileLot>.To(c => new LabReportChileLotReturn
                        {
                            WeightedAttributes = c.Lot.Attributes.Select(a => new WeightedLotAttributeReturn
                                {
                                    HasResolvedDefects = c.Lot.AttributeDefects.Any(d => d.AttributeShortName == a.AttributeShortName) &&
                                        c.Lot.AttributeDefects.Where(d => d.AttributeShortName == a.AttributeShortName).All(d => d.LotDefect.Resolution != null)
                                })
                        }),

                    Projector<ChileLot>.To(c => new LabReportChileLotReturn
                        {
                            ChileProductKeyReturn = chileProductKey.Invoke(c.ChileProduct.Product),
                            PackScheduleBaseReturn = productionBatches.Where(b => b.LotDateCreated == c.LotDateCreated && b.LotDateSequence == c.LotDateSequence && b.LotTypeId == c.LotTypeId)
                                .Select(b => packSchedule.Invoke(b.PackSchedule)).FirstOrDefault()
                        }),

                    Projector<ChileLot>.To(c => new LabReportChileLotReturn
                        {
                            ProductionResultBaseReturn = lotProdcutionResuls.Where(r => r.LotDateCreated == c.LotDateCreated && r.LotDateSequence == c.LotDateSequence && r.LotTypeId == c.LotTypeId)
                                .Select(r => productionResult.Invoke(r)).FirstOrDefault(),
                            PickedLots = chileLotProduction.Where(r => r.LotDateCreated == c.LotDateCreated && r.LotDateSequence == c.LotDateSequence && r.LotTypeId == c.LotTypeId)
                                .SelectMany(r => r.PickedInventory.Items.Select(i => lotTotes.Invoke(i)))
                        })
                };
        }

        internal static Expression<Func<Lot, LotSelect>> LotSelect()
        {
            return Projector<Lot>.To(l => new LotSelect
                {
                    Lot = l,
                    ChileLot = l.ChileLot,
                    AdditiveLot = l.AdditiveLot,
                    PackagingLot = l.PackagingLot
                });
        }

        internal static Expression<Func<Lot, Lot, bool>> SelectLotsAreEqual()
        {
            return (a, b) => a.LotDateCreated == b.LotDateCreated && a.LotDateSequence == b.LotDateSequence && a.LotTypeId == b.LotTypeId;
        }

        internal static Expression<Func<Lot, LotInputTraceSelect>> SelectLotInputTrace()
        {
            var lotKey = SelectLotKey<Lot>();

            return Projector<Lot>.To(l => new LotInputTraceSelect
                {
                    LotKey = lotKey.Invoke(l),
                    Inputs = new[] { l.ChileLot.Production }.Where(p => p != null).SelectMany(p => p.PickedInventory.Items
                        .GroupBy(i => i.Lot)
                        .Select(i => new LotInputTracePickedSelect
                            {
                                PickedLotKey = lotKey.Invoke(i.Key),
                                PickedTreatments = i.Select(n => n.Treatment.ShortName).Distinct()
                            }))
                });
        }

        internal static Expression<Func<Lot, LotOutputTraceSelect>> SelectLotOutputTrace(ILotUnitOfWork lotUniOfWork)
        {
            var lotKey = SelectLotKey<Lot>();
            var productionLotKey = SelectLotKey<ChileLotProduction>();

            var productions = lotUniOfWork.ChileLotProductionRepository.SourceQuery;
            var salesOrders = lotUniOfWork.SalesOrderRepository.SourceQuery;

            return Projector<Lot>.To(l => new LotOutputTraceSelect
                {
                    LotKey = lotKey.Invoke(l),
                    ProductionOutput = l.PickedInventory
                        .GroupBy(i => i.PickedInventory)
                        .Join(productions,
                            p => p.Key,
                            r => r.PickedInventory,
                            (p, r) => new LotOutputTraceProductionSelect
                                {
                                    DestinationLot = productionLotKey.Invoke(r),
                                    PickedTreatments = p.Select(n => n.Treatment.ShortName).Distinct()
                                }),
                    OrderOutput = l.PickedInventory
                        .GroupBy(i => i.PickedInventory)
                        .Join(salesOrders,
                            p => p.Key,
                            o => o.InventoryShipmentOrder.PickedInventory,
                            (p, o) => new LotOutputTraceOrderSelect
                                {
                                    MoveNum = o.InventoryShipmentOrder.MoveNum,
                                    ShipmentDate = o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate,
                                    CustomerName = new [] { o.Customer }.Where(c => c != null).Select(c => c.Company.Name).FirstOrDefault(),
                                    PickedTreatments = p.Select(n => n.Treatment.ShortName).Distinct()
                                })
                });
        }

        internal static IEnumerable<Expression<Func<Lot, LotHistoryReturn>>> SplitSelectHistory()
        {
            var lotKey = SelectLotKey<Lot>();
            var product = SelectDerivedProduct();
            var attributes = LotAttributeProjectors.SelectHistory();
            var history = LotHistoryProjectors.Select();
            var employee = EmployeeProjectors.SelectSummary();

            return new Projectors<Lot, LotHistoryReturn>
                {
                    l => new LotHistoryReturn
                        {
                            LotKeyReturn = lotKey.Invoke(l),
                            Timestamp = l.TimeStamp,
                            Employee = employee.Invoke(l.Employee),

                            LoBac = new [] { l.ChileLot }.Where(c => c != null).Select(c => c.AllAttributesAreLoBac).DefaultIfEmpty(true).FirstOrDefault(),
                            HoldType = l.Hold,
                            HoldDescription = l.HoldDescription,
                            QualityStatus = l.QualityStatus,
                            ProductionStatus = l.ProductionStatus,
                            Product = product.Invoke(l)
                        },
                    l => new LotHistoryReturn
                        {
                            Attributes = l.Attributes.Select(a => attributes.Invoke(a))
                        },
                    l => new LotHistoryReturn
                        {
                            SerializedHistory = l.History.Select(h => history.Invoke(h))
                        }
                };
        }

        #region Private Parts

        private static Expression<Func<Lot, LotBaseReturn>> SelectLotBase()
        {
            var lotKey = SelectLotKey<Lot>();

            return l => new LotBaseReturn
                {
                    LotKeyReturn = lotKey.Invoke(l),
                    HoldType = l.Hold,
                    HoldDescription = l.HoldDescription,
                    QualityStatus = l.QualityStatus,
                    ProductionStatus = l.ProductionStatus,
                    Notes = l.Notes
                };
        }

        private static Expression<Func<ChileLot, InventoryProductReturn>> SelectInventoryChileProductReturn()
        {
            return c => new InventoryProductReturn
                {
                    ProductId = c.ChileProduct.Product.Id,
                    ProductName = c.ChileProduct.Product.Name,
                    ProductType = c.ChileProduct.Product.ProductType,
                    ProductSubType = c.ChileProduct.ChileType.Description,
                    IsActive = c.ChileProduct.Product.IsActive,
                    ProductCode = c.ChileProduct.Product.ProductCode,
                };
        }

        private static Expression<Func<AdditiveLot, InventoryProductReturn>> SelectInventoryAdditiveProductReturn()
        {
            return a => new InventoryProductReturn
                {
                    ProductId = a.AdditiveProduct.Product.Id,
                    ProductName = a.AdditiveProduct.Product.Name,
                    ProductType = a.AdditiveProduct.Product.ProductType,
                    ProductSubType = a.AdditiveProduct.AdditiveType.Description,
                    IsActive = a.AdditiveProduct.Product.IsActive,
                    ProductCode = a.AdditiveProduct.Product.ProductCode,
                };
        }

        private static Expression<Func<PackagingLot, InventoryProductReturn>> SelectInventoryPackagingProductReturn()
        {
            return p => new InventoryProductReturn
                {
                    ProductId = p.PackagingProduct.Product.Id,
                    ProductName = p.PackagingProduct.Product.Name,
                    ProductType = p.PackagingProduct.Product.ProductType,
                    ProductSubType = "Packaging",
                    IsActive = p.PackagingProduct.Product.IsActive,
                    ProductCode = p.PackagingProduct.Product.ProductCode,
                };
        }

        private static Expression<Func<LotAttribute, DateTime?, int?>> CalculateAsta(DateTime currentDate)
        {
            var astaCalc = AstaCalculator.CalculateAsta();

            return (a, d) => astaCalc.Invoke(a.AttributeValue, a.AttributeDate, d ?? a.AttributeDate, currentDate);
        }

        #endregion
    }
}

// ReSharper restore RedundantEmptyObjectOrCollectionInitializer
// ReSharper restore ConvertClosureToMethodGroup