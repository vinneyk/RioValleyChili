using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class LotPredicates
    {
        internal static Expression<Func<Lot, bool>> FilterByLotDateAndTypeId(DateTime lotDate, LotTypeEnum lotType)
        {
            return l => l.LotDateCreated == lotDate && l.LotTypeId == (int)lotType;
        }

        internal static Expression<Func<ChileLot, bool>> AvailableChileLotsFilter
        {
            get { return c => c.Lot.Inventory.Any(i => i.Quantity > 0); }
        }

        internal static Expression<Func<AdditiveLot, bool>> AvailableAdditiveLotsFilter
        {
            get { return l => l.Lot.Inventory.Any(i => i.Quantity > 0); }
        }

        internal static Expression<Func<PackagingLot, bool>> AvailablePackagingLotsFilter
        {
            get { return p => p.Lot.Inventory.Any(i => i.Quantity > 0); }
        }

        internal static Expression<Func<Lot, bool>> FilterByProductType(ILotUnitOfWork lotUnitOfWork, ProductTypeEnum productType)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }

            var product = LotProjectors.SelectProduct(lotUnitOfWork);

            return l => product.Invoke(l).ProductType == productType;
        }

        internal static Expression<Func<Lot, bool>> FilterByLotType(LotTypeEnum lotType)
        {
            var lotTypeId = (int) lotType;
            return l => l.LotTypeId == lotTypeId;
        }

        internal static Expression<Func<Lot, bool>> FilterByLotProductionStart(ILotUnitOfWork lotUnitOfWork, DateTime? rangeStart, DateTime? rangeEnd)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            
            var results = LotProjectors.SelectChileLotResults(lotUnitOfWork);
            var byProductionBegin = ProductionResultPredicates.FilterByProductionBegin(rangeStart, rangeEnd);
            return l => results.Invoke(l).Select(r => byProductionBegin.Invoke(r)).FirstOrDefault() ?? false;
        }

        internal static Expression<Func<Lot, bool>> FilterByProductionStatus(LotProductionStatus productionStatus)
        {
            return l => l.ProductionStatus == productionStatus;
        }

        internal static Expression<Func<Lot, bool>> FilterByQualityStatus(LotQualityStatus lotStatus)
        {
            return l => l.QualityStatus == lotStatus;
        }

        internal static Expression<Func<Lot, bool>> FilterByProductSpecComplete(bool productSpecComplete)
        {
            return l => l.ProductSpecComplete == productSpecComplete;
        }

        internal static Expression<Func<Lot, bool>> FilterByProductSpecOutOfRange(bool productSpecOutOfRange)
        {
            return l => l.ProductSpecOutOfRange == productSpecOutOfRange;
        }

        internal static Expression<Func<ChileLot, bool>> FilterByAttributeDateForLabReport(DateTime minTestDate, DateTime maxTestDate)
        {
            return c => c.Lot.Attributes.Any(a => !a.Computed && a.AttributeDate >= minTestDate && a.AttributeDate <= maxTestDate);
        }

        internal static Expression<Func<ChileLot, bool>> FilterForLabReport()
        {
            return c => c.LotTypeId == (int) LotTypeEnum.WIP || c.LotTypeId == (int) LotTypeEnum.FinishedGood;
        }

        public static Expression<Func<Lot, bool>> FilterByStartingLotKey(ILotKey lotKey)
        {
            return l => l.LotTypeId == lotKey.LotKey_LotTypeId 
                && ((l.LotDateCreated == lotKey.LotKey_DateCreated && l.LotDateSequence >= lotKey.LotKey_DateSequence) || (l.LotDateCreated > lotKey.LotKey_DateCreated));
        }

        public static Expression<Func<TLotDerivative0, Expression<Func<TLotDerivative1, bool>>>> 
            ConstructLotKeyPredicate<TLotDerivative0, TLotDerivative1>()
            where TLotDerivative0 : LotKeyEntityBase
            where TLotDerivative1 : LotKeyEntityBase
        {
            return a => b =>
                a.LotDateCreated == b.LotDateCreated
                && a.LotDateSequence == b.LotDateSequence
                && a.LotTypeId == b.LotTypeId;
        }

        public static Expression<Func<TLot, bool>> ConstructPredicate<TLot>(ILotKey lotKey)
            where TLot : LotKeyEntityBase
        {
            return l => l.LotTypeId == lotKey.LotKey_LotTypeId && l.LotDateCreated == lotKey.LotKey_DateCreated && l.LotDateSequence == lotKey.LotKey_DateSequence;
        }

        public static Expression<Func<Lot, bool>> FilterByProductKey(ILotUnitOfWork lotUnitOfWork, IKey<Product> productKey)
        {
            var productPredicate = productKey.FindByPredicate;
            var product = LotProjectors.SelectProduct(lotUnitOfWork);
            return l => productPredicate.Invoke(product.Invoke(l));
        }
    }
}