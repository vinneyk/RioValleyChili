// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class PickedInventoryItemProjectors
    {
        internal static Expression<Func<PickedInventoryItem, PickedInventoryItemKeyReturn>> SelectKey()
        {
            return i => new PickedInventoryItemKeyReturn
                {
                    PickedInventoryKey_DateCreated = i.DateCreated,
                    PickedInventoryKey_Sequence = i.Sequence,
                    PickedInventoryItemKey_Sequence = i.ItemSequence
                };
        }

        internal static Expression<Func<PickedInventoryItem, InventoryKeyReturn>> SelectInventoryKey()
        {
            return i => new InventoryKeyReturn
                {
                    LotKey_DateCreated = i.LotDateCreated,
                    LotKey_DateSequence = i.LotDateSequence,
                    LotKey_LotTypeId = i.LotTypeId,
                    PackagingProductKey_ProductId = i.PackagingProductId,
                    LocationKey_Id = i.FromLocationId,
                    InventoryTreatmentKey_Id = i.TreatmentId,
                    InventoryKey_ToteKey = i.ToteKey
                };
        }

        internal static Expression<Func<PickedInventoryItem, PickedInventoryItemSelect>> ItemSelect()
        {
            var lotSelect = LotProjectors.LotSelect();
            return Projector<PickedInventoryItem>.To(i => new PickedInventoryItemSelect
                {
                    Item = i,
                    LotSelect = lotSelect.Invoke(i.Lot),
                });
        }

        [Issue("Need to manually add transactions for PickedInventoryItems for ProductionBatches that have not had results entered.",
            Todo = "Remove manual addition of transaction reocrds once they are being logged while being picked. -RI 2016-06-14",
            References = new[] { "RVCADMIN-1153" },
            Flags = IssueFlags.TodoWhenAccessFreedom)]
        internal static Expression<Func<PickedInventoryItem, ProductionBatch, PickedForBatchTransactionReturn>> SelectTransaction(ILotUnitOfWork lotUnitOfWork)
        {
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var batchLotKey = LotProjectors.SelectLotKey<ProductionBatch>();
            var packScheduleKey = PackScheduleProjectors.SelectKey();

            var product = LotProjectors.SelectDerivedProduct();
            var location = LocationProjectors.SelectLocation();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();
            var packaging = ProductProjectors.SelectPackagingProduct();

            return Projector<PickedInventoryItem, ProductionBatch>.To((i, b) => new PickedForBatchTransactionReturn
                {
                    EmployeeName = i.PickedInventory.Employee.UserName,
                    TimeStamp = i.PickedInventory.TimeStamp,

                    TransactionType = InventoryTransactionType.PickedForBatch,
                    Quantity = -i.Quantity,
                    Weight = -i.Quantity * i.PackagingProduct.Weight,
                    ToteKey = i.ToteKey,

                    SourceLotVendorName = new[] { i.Lot.Vendor }.Where(v => v != null).Select(v => v.Name).FirstOrDefault(),
                    SourceLotPurchaseOrderNumber = i.Lot.PurchaseOrderNumber,
                    SourceLotShipperNumber = i.Lot.ShipperNumber,

                    Product = product.Invoke(i.Lot),
                    Packaging = packaging.Invoke(i.PackagingProduct),
                    Location = location.Invoke(i.FromLocation),
                    Treatment = treatment.Invoke(i.Treatment),
                    
                    SourceLotKeyReturn = lotKey.Invoke(i.Lot),
                    DestinationLotKeyReturn = batchLotKey.Invoke(b),
                    PackScheduleKeyReturn = packScheduleKey.Invoke(b.PackSchedule)
                });
        }

        #region SplitSelect

        internal static IEnumerable<Expression<Func<PickedInventoryItem, PickedInventoryItemReturn>>> SplitSelect(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, ISalesUnitOfWork salesUnitOfWork = null)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var pickedInventoryItemKey = SelectKey();
            var inventoryKey = SelectInventoryKey();
            var inventoryQuantity = SelectInventoryQuantity();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();
            var location = LocationProjectors.SelectLocation();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            
            var results = LotProjectors.SplitSelectLotSummary(inventoryUnitOfWork, currentDate)
                .Select(p => p.Merge((Expression<Func<PickedInventoryItem, PickedInventoryItemReturn>>) (n => new PickedInventoryItemReturn {}), n => n.Lot))
                .ToListWithModifiedElement(0, p => p.Merge(i => new PickedInventoryItemReturn
                    {
                        PickedInventoryItemKeyReturn = pickedInventoryItemKey.Invoke(i),
                        InventoryKeyReturn = inventoryKey.Invoke(i),
                        ToteKey = i.ToteKey,
                        QuantityPicked = i.Quantity,
                        CustomerLotCode = i.CustomerLotCode,
                        CustomerProductCode = i.CustomerProductCode
                    }))
                .ToAppendedList(i => new PickedInventoryItemReturn
                    {
                        PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                        Location = location.Invoke(i.FromLocation),
                        InventoryTreatment = treatment.Invoke(i.Treatment),
                    },
                    i => new PickedInventoryItemReturn
                    {
                        PackagingReceived = packagingProduct.Invoke(i.Lot.ReceivedPackaging),
                        Quantity = inventoryQuantity.Invoke(i),
                        CurrentLocation = location.Invoke(i.CurrentLocation),
                    });

            if(salesUnitOfWork != null)
            {
                var orderItemKey = InventoryPickOrderItemProjectors.SelectKey();
                var customerPickedItems = salesUnitOfWork.SalesOrderPickedItemRepository.All();
                results.Add(i => new PickedInventoryItemReturn
                    {
                        PickOrderItemKeyReturn = customerPickedItems
                            .Where(c => c.DateCreated == i.DateCreated && c.Sequence == i.Sequence && c.ItemSequence == i.ItemSequence)
                            .Select(c => orderItemKey.Invoke(c.SalesOrderItem.InventoryPickOrderItem))
                            .FirstOrDefault()
                    });
            }

            return results.Select(p => p.ExpandAll());
        }

        internal static IEnumerable<Expression<Func<PickedInventoryItem, BatchPickedInventoryItemReturn>>> SplitSelectBatch(IProductionUnitOfWork productionUnitOfWork, DateTime currentDate)
        {
            var batches = productionUnitOfWork.ProductionBatchRepository.All();
            var lotKey = LotProjectors.SelectLotKey<ProductionBatch>();

            return SplitSelect(productionUnitOfWork, currentDate)
                .Select(p => p.Merge(i => new BatchPickedInventoryItemReturn { }))
                .ToAppendedList(i => new BatchPickedInventoryItemReturn
                    {
                        NewLotKeyReturn = lotKey.Invoke(batches.FirstOrDefault(b => b.Production.PickedInventoryDateCreated == i.DateCreated && b.Production.PickedInventorySequence == i.Sequence))
                    });
        }

        internal static IEnumerable<Expression<Func<PickedInventoryItem, PickSheetItemReturn>>>  SplitSelectPickSheetItem()
        {
            var key = SelectKey();
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var locationKey = LocationProjectors.SelectLocationKey();
            var loBac = LotProjectors.SelectLoBac();
            var lotProduct = LotProjectors.SelectDerivedProduct();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return new Projectors<PickedInventoryItem, PickSheetItemReturn>
                {
                    i => new PickSheetItemReturn
                        {
                            LocationKeyReturn = locationKey.Invoke(i.FromLocation),
                            Description = i.FromLocation.Description,
                            PickedInventoryItemKeyReturn = key.Invoke(i),
                            LotKeyReturn = lotKey.Invoke(i.Lot),
                            Quantity = i.Quantity,
                            LoBac = loBac.Invoke(i.Lot),
                            CustomerProductCode = i.CustomerProductCode
                        },
                    i => new PickSheetItemReturn
                        {
                            LotProduct = lotProduct.Invoke(i.Lot),
                            InventoryTreatment = treatment.Invoke(i.Treatment)
                        },
                    i => new PickSheetItemReturn
                        {
                            PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                            NetWeight = i.Quantity * i.PackagingProduct.Weight
                        }
                };
        }

        internal static IEnumerable<Expression<Func<PickedInventoryItem, PackingListPickedInventoryItemReturn>>>  SplitSelectPackingListItem()
        {
            return new Projectors<PickedInventoryItem, PackingListPickedInventoryItemReturn>
                {
                    { SplitSelectPickSheetItem(), p => p.Merge(o => new PackingListPickedInventoryItemReturn { }) },
                    i => new PackingListPickedInventoryItemReturn
                        {
                            CustomerProductCode = i.CustomerProductCode,
                            CustomerLotCode = i.CustomerLotCode,
                            GrossWeight = i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight)
                        }
                };
        }

        internal static IEnumerable<Expression<Func<PickedInventoryItem, InventoryShipmentOrderItemAnalysisReturn>>> SplitSelectCOAItem()
        {
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var loBac = LotProjectors.SelectLoBac();
            var lotProduct = LotProjectors.SelectDerivedProduct();
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();
            var attribute = LotAttributeProjectors.Select();

            return new Projectors<PickedInventoryItem, InventoryShipmentOrderItemAnalysisReturn>
                {
                    i => new InventoryShipmentOrderItemAnalysisReturn
                        {
                            LotKeyReturn = lotKey.Invoke(i.Lot),
                            TreatmentReturn = treatment.Invoke(i.Treatment),
                            Attributes = i.Lot.Attributes.Select(a => attribute.Invoke(a)),
                            Notes = i.Lot.Notes
                        },
                    i => new InventoryShipmentOrderItemAnalysisReturn
                        {
                            ProductionDate = new [] { i.Lot.ChileLot }.Where(c => c != null && c.Production != null && c.Production.Results != null)
                                .Select(c => (DateTime?)c.Production.Results.ProductionEnd).FirstOrDefault(),
                        },
                    i => new InventoryShipmentOrderItemAnalysisReturn
                        {
                            LoBac = loBac.Invoke(i.Lot),
                            LotProduct = lotProduct.Invoke(i.Lot),
                        }
                };
        }

        #endregion

        internal static Expression<Func<PickedInventoryItem, PickedInventoryItemReturn>> SelectMillAndWetdownPickedItem(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var pickedInventoryItemKey = SelectKey();
            var inventoryKey = SelectInventoryKey();
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var lotProduct = LotProjectors.SelectDerivedProduct();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            var location = LocationProjectors.SelectLocation();

            return i => new PickedInventoryItemReturn
                {
                    PickedInventoryItemKeyReturn = pickedInventoryItemKey.Invoke(i),
                    InventoryKeyReturn = inventoryKey.Invoke(i),
                    LotKeyReturn = lotKey.Invoke(i.Lot),
                    LotDateCreated = i.LotDateCreated,
                    ToteKey = i.ToteKey,
                    QuantityPicked = i.Quantity,
                    CustomerLotCode = i.CustomerLotCode,
                    CustomerProductCode = i.CustomerProductCode,
                    TotalWeightPicked = (int) (i.PackagingProduct.Weight * i.Quantity),

                    LotProduct = lotProduct.Invoke(i.Lot),
                    PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                    Location = location.Invoke(i.FromLocation)
                };
        }

        internal static Expression<Func<PickedInventoryItem, PickedLotReturn>> SelectPickedLot()
        {
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var attribute = LotAttributeProjectors.SelectParameter();

            return i => new PickedLotReturn
                {
                    LotKey = lotKey.Invoke(i.Lot),
                    ToteKey = i.ToteKey,
                    LotWeight = i.PackagingProduct.Weight * i.Quantity,
                    LotAttributes = i.Lot.Attributes.Select(a => attribute.Invoke(a))
                };
        }

        private static Expression<Func<PickedInventoryItem, int>> SelectInventoryQuantity()
        {
            return p => p.Lot.Inventory.Where(i => i.PackagingProductId == p.PackagingProductId && i.LocationId == p.FromLocationId && i.TreatmentId == p.TreatmentId)
                         .Select(i => i != null ? i.Quantity : 0).FirstOrDefault();
        }
    }
}
// ReSharper restore ConvertClosureToMethodGroup