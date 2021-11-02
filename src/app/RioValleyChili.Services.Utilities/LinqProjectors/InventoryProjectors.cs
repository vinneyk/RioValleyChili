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
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryProjectors
    {
        internal static Expression<Func<Inventory, InventoryKeyReturn>> SelectInventoryKey()
        {
            return i => new InventoryKeyReturn
                {
                    LotKey_DateCreated = i.LotDateCreated,
                    LotKey_DateSequence = i.LotDateSequence,
                    LotKey_LotTypeId = i.LotTypeId,
                    PackagingProductKey_ProductId = i.PackagingProductId,
                    LocationKey_Id = i.LocationId,
                    InventoryTreatmentKey_Id = i.TreatmentId,
                    InventoryKey_ToteKey = i.ToteKey
                };
        }

        internal static Expression<Func<Inventory, Product>> InventoryToProduct(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            var lotToProduct = LotProjectors.SelectProduct(inventoryUnitOfWork);

            return i => lotToProduct.Invoke(i.Lot);
        }

        #region SelectInventorySummary

        internal static IEnumerable<Expression<Func<Inventory, InventoryItemReturn>>> SplitSelectInventorySummary(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            currentDate = currentDate.Date;
            var inventoryKey = SelectInventoryKey();
            var packagingProduct = ProductProjectors.SelectPackagingProduct();
            var warehouseLocation = LocationProjectors.SelectLocation();
            var treament = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return LotProjectors.SplitSelectLotSummary(inventoryUnitOfWork, currentDate)
                .Select(p => p.Merge(Projector<Inventory>.To(n => new InventoryItemReturn { }), n => n.Lot))
                .ToListWithModifiedElement(0, p => p.Merge(n => new InventoryItemReturn
                    {
                        InventoryKeyReturn = inventoryKey.Invoke(n),
                        ToteKey = n.ToteKey,
                        Quantity = n.Quantity
                    }).ExpandAll())
                .ToAppendedList(Projector<Inventory>.To(i => new InventoryItemReturn
                    {
                        PackagingReceived = packagingProduct.Invoke(i.Lot.ReceivedPackaging),
                        PackagingProduct = packagingProduct.Invoke(i.PackagingProduct),
                        Location = warehouseLocation.Invoke(i.Location),
                        InventoryTreatment = treament.Invoke(i.Treatment)
                    }));
        }

        internal static IEnumerable<Expression<Func<Inventory, PickableInventoryItemReturn>>> SplitSelectPickableInventorySummary(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate,
            IEnumerable<Expression<Func<Inventory, bool>>> validForPicking, bool includeAllowances)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var selectValidForPicking = validForPicking.Aggregate(PredicateBuilder.True<Inventory>(), (c, n) => c.And(n)).ExpandAll();

            var projectors = SplitSelectInventorySummary(inventoryUnitOfWork, currentDate)
                .Select(p => p.Merge(Projector<Inventory>.To(n => new PickableInventoryItemReturn { })))
                .ToAppendedList(Projector<Inventory>.To(n => new PickableInventoryItemReturn
                    {
                        ValidForPicking = selectValidForPicking.Invoke(n)
                    }));

            if(includeAllowances)
            {
                var contractAllowance = LotContractAllowanceProjectors.SelectContractKey();
                var customerOrderAllowance = LotCustomerOrderAllowanceProjectors.SelectCustomerOrderKey();
                var customerAllowance = LotCustomerAllowanceProjectors.SelectCustomerKey();

                projectors = projectors.ToAppendedList(Projector<Inventory>.To(i => new PickableInventoryItemReturn
                    {
                        ContractAllowances = i.Lot.ContractAllowances.Select(a => contractAllowance.Invoke(a)),
                        CustomerOrderAllowances = i.Lot.SalesOrderAllowances.Select(a => customerOrderAllowance.Invoke(a)),
                        CustomerAllowances = i.Lot.CustomerAllowances.Select(a => customerAllowance.Invoke(a))
                    }));
            }

            return projectors;
        }

        #endregion
    }
}

// ReSharper restore ConvertClosureToMethodGroup