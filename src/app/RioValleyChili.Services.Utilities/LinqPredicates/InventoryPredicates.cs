// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Utilities.LinqProjectors;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class InventoryPredicates
    {
        internal static Expression<Func<Inventory, bool>> ByLotKey(IKey<Lot> lotKey)
        {
            var lotKeyPredicate = lotKey.FindByPredicate;

            return i => lotKeyPredicate.Invoke(i.Lot);
        }

        internal static Expression<Func<Inventory, bool>> ByProductKey(IKey<Product> productKey, IInventoryUnitOfWork inventoryUnitOfWork)
        {
            var inventoryToProduct = InventoryProjectors.InventoryToProduct(inventoryUnitOfWork);
            var productKeyPredicate = productKey.FindByPredicate;

            return i => productKeyPredicate.Invoke(inventoryToProduct.Invoke(i));
        }

        internal static Expression<Func<Inventory, bool>> ByPackaging(IKey<PackagingProduct> packagingProductKey)
        {
            var packagingProduct = packagingProductKey.FindByPredicate;

            return i => packagingProduct.Invoke(i.PackagingProduct);
        }

        internal static Expression<Func<Inventory, bool>> ByPackagingReceived(IKey<PackagingProduct> packagingProductKey)
        {
            var packagingProduct = packagingProductKey.FindByPredicate;

            return i => packagingProduct.Invoke(i.Lot.ReceivedPackaging);
        }

        internal static Expression<Func<Inventory, bool>> ByLocationKey(IKey<Location> locationKey)
        {
            var location = locationKey.FindByPredicate;

            return i => location.Invoke(i.Location);
        }

        internal static Expression<Func<Inventory, bool>> ByProductType(ProductTypeEnum productType, IInventoryUnitOfWork inventoryUnitOfWork)
        {
            var inventoryToProduct = InventoryProjectors.InventoryToProduct(inventoryUnitOfWork);

            return i => inventoryToProduct.Invoke(i).ProductType == productType;
        }

        internal static Expression<Func<Inventory, bool>> ByTreatment(IKey<InventoryTreatment> treatmentKey)
        {
            var treatment = treatmentKey.FindByPredicate;

            return i => treatment.Invoke(i.Treatment);
        }

        internal static Expression<Func<Inventory, bool>> ByFacilityKey(IKey<Facility> warehouseKey)
        {
            var facilityKeyPredicate = warehouseKey.FindByPredicate;
            
            return i => facilityKeyPredicate.Invoke(i.Location.Facility);
        }

        internal static Expression<Func<Inventory, bool>> ByLotType(LotTypeEnum lotTypeEnum)
        {
            var lotType = (int) lotTypeEnum;
            
            return i => i.LotTypeId == lotType;
        }

        internal static Expression<Func<Inventory, bool>> ByLotProductionStatus(params LotProductionStatus[] validProductionStatuses)
        {
            return i => validProductionStatuses.Contains(i.Lot.ProductionStatus);
        }

        internal static Expression<Func<Inventory, bool>> ByLotStatus(params LotQualityStatus[] validQualityStatuses)
        {
            return i => validQualityStatuses.Contains(i.Lot.QualityStatus);
        }

        internal static Expression<Func<Inventory, bool>> ByLotOnHold(bool onHold)
        {
            return i => (i.Lot.Hold != null) == onHold;
        }

        internal static Expression<Func<Inventory, bool>> ByLotHoldType(LotHoldType holdType)
        {
            return i => i.Lot.Hold == holdType;
        }

        internal static Expression<Func<Inventory, bool>> ByToteKeys(List<string> toteKeys)
        {
            return i => toteKeys.Contains(i.ToteKey);
        }

        internal static Expression<Func<Inventory, bool>> ByToteKey(string toteKey)
        {
            return i => toteKey == i.ToteKey;
        }

        internal static Expression<Func<Inventory, bool>> ByLocationLocked(bool locked)
        {
            return i => i.Location.Locked == locked;
        }

        internal static Expression<Func<Inventory, bool>> ByLocationDescription(string description)
        {
            var streetPrefix = string.Format("{0}{1}", description, LocationDescriptionHelper.Separator);
            return i => i.Location.Description == description || i.Location.Description.StartsWith(streetPrefix);
        }

        internal static Expression<Func<Inventory, bool>> ByAdditiveTypeKey(IKey<AdditiveType> additiveTypeKey)
        {
            var additiveType = additiveTypeKey.FindByPredicate;

            return i => new[] { i.Lot.AdditiveLot }.Where(a => a != null).Select(a => a.AdditiveProduct.AdditiveType).Any(t => additiveType.Invoke(t));
        }

        internal static Expression<Func<Inventory, bool>> ByAttributeRanges(IDictionary<IAttributeNameKey, IAttributeRange> ranges)
        {
            return ranges == null || !ranges.Any() ? i => true : ranges.Aggregate((Expression<Func<Inventory, bool>>) null, (c, n) =>
                {
                    var attributePredicate = LotAttributePredicates.ByAttributeNameKey(n.Key);
                    var predicate = Projector<Inventory>.To(i => i.Lot.Attributes.Any(a => attributePredicate.Invoke(a) && a.AttributeValue >= n.Value.RangeMin && a.AttributeValue <= n.Value.RangeMax));
                    return c == null ? predicate : c.And(predicate);
                });
        }

        internal static Expression<Func<Inventory, bool>> ByAllowance(ICustomerKey customer, ISalesOrderKey salesOrder, IContractKey contract)
        {
            var predicate = PredicateBuilder.False<Inventory>();
            if(contract != null)
            {
                var contractPredicate = LotContractAllowancePredicates.ByContractKey(contract);
                predicate = predicate.Or(i => i.Lot.ContractAllowances.Any(a => contractPredicate.Invoke(a)));
            }

            if(salesOrder != null)
            {
                var customerOrderPredicate = LotCustomerOrderAllowancePredicates.ByCustomerOrderKey(salesOrder);
                predicate = predicate.Or(i => i.Lot.SalesOrderAllowances.Any(a => customerOrderPredicate.Invoke(a)));
            }

            if(customer != null)
            {
                var customerPredicate = LotCustomerAllowancePredicates.ByCustomerKey(customer);
                predicate = predicate.Or(i => i.Lot.CustomerAllowances.Any(a => customerPredicate.Invoke(a)));
            }

            return predicate;
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup