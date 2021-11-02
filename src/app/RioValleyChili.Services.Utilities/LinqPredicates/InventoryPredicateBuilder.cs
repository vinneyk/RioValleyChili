using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class InventoryPredicateBuilder
    {
        internal static IResult<Expression<Func<Inventory, bool>>> BuildPredicate(IInventoryUnitOfWork inventoryUnitOfWork, PredicateBuilderFilters filters, params Expression<Func<Inventory, bool>>[] predicates)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var predicate = PredicateBuilder.True<Inventory>();
            if(predicates != null)
            {
                predicate = predicates.Aggregate(predicate, (c, p) => c.And(p.ExpandAll()));
            }

            return ApplyFilters(predicate, filters, inventoryUnitOfWork);
        }

        internal static IResult<Expression<Func<Inventory, bool>>> BuildPredicate(IInventoryUnitOfWork inventoryUnitOfWork, PredicateBuilderFilters filters, IEnumerable<Expression<Func<Inventory, bool>>> predicates)
        {
            return BuildPredicate(inventoryUnitOfWork, filters, predicates == null ? null : predicates.ToArray());
        }

        private static IResult<Expression<Func<Inventory, bool>>> ApplyFilters(Expression<Func<Inventory, bool>> predicate, PredicateBuilderFilters filters, IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(filters != null)
            {
                if(filters.LotKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByLotKey(filters.LotKey.ToLotKey()).ExpandAll());
                }

                if(filters.ProductKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByProductKey(filters.ProductKey.ToProductKey(), inventoryUnitOfWork).ExpandAll());
                }

                if(filters.FacilityKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByFacilityKey(filters.FacilityKey.ToFacilityKey()).ExpandAll());
                }

                if(filters.ProductType != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByProductType(filters.ProductType.Value, inventoryUnitOfWork).ExpandAll());
                }

                if(filters.LotType != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByLotType(filters.LotType.Value).ExpandAll());
                }

                if(filters.HoldType != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByLotHoldType(filters.HoldType.Value).ExpandAll());
                }

                if(filters.ToteKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByToteKey(filters.ToteKey).ExpandAll());
                }

                if(filters.AdditiveTypeKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByAdditiveTypeKey(filters.AdditiveTypeKey.ToAdditiveTypeKey()).ExpandAll());
                }

                if(filters.TreatmentKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByTreatment(filters.TreatmentKey.ToInventoryTreatmentKey()).ExpandAll());
                }

                if(filters.PackagingKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByPackaging(filters.PackagingKey.ToPackagingProductKey()).ExpandAll());
                }

                if(filters.PackagingReceivedKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByPackagingReceived(filters.PackagingReceivedKey.ToPackagingProductKey()).ExpandAll());
                }

                if(filters.LocationKey != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByLocationKey(filters.LocationKey.ToLocationKey()).ExpandAll());
                }

                if(filters.LocationGroupName != null)
                {
                    predicate = predicate.And(InventoryPredicates.ByLocationDescription(filters.LocationGroupName).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<Inventory, bool>>>(predicate.ExpandAll());
        }

        internal class PredicateBuilderFilters
        {
            internal ILotKey LotKey = null;
            internal IProductKey ProductKey = null;
            internal ProductTypeEnum? ProductType = null;
            internal LotTypeEnum? LotType = null;
            internal LotHoldType? HoldType = null;
            internal string ToteKey = null;
            internal IAdditiveTypeKey AdditiveTypeKey = null;
            internal IFacilityKey FacilityKey = null;
            internal IInventoryTreatmentKey TreatmentKey = null;
            internal IPackagingProductKey PackagingKey = null;
            internal IPackagingProductKey PackagingReceivedKey = null;
            internal ILocationKey LocationKey = null;
            internal string LocationGroupName;
        }
    }
}