using System;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class InventoryTransactionPredicates
    {
        internal static Expression<Func<InventoryTransaction, bool>> ByTransactionType(InventoryTransactionType transactionType)
        {
            return t => t.TransactionType == transactionType;
        }

        internal static Expression<Func<InventoryTransaction, bool>> BySourceLot(IKey<Lot> lotKey)
        {
            var predicate = lotKey.FindByPredicate;
            return Projector<InventoryTransaction>.To(i => predicate.Invoke(i.SourceLot));
        }

        internal static Expression<Func<InventoryTransaction, bool>> ByDestinationLot(IKey<Lot> lotKey)
        {
            var predicate = lotKey.FindByPredicate;
            return Projector<InventoryTransaction>.To(i => predicate.Invoke(i.DestinationLot));
        }

        internal static Expression<Func<InventoryTransaction, bool>> ByInventoryKey(IInventoryKey inventoryKey)
        {
            var lot = new LotKey(inventoryKey).FindByPredicate;
            var location = new LocationKey(inventoryKey).FindByPredicate;
            var packaging = new PackagingProductKey(inventoryKey).FindByPredicate;
            var treatment = new InventoryTreatmentKey(inventoryKey).FindByPredicate;

            return Projector<InventoryTransaction>.To(i =>
                lot.Invoke(i.SourceLot) &&
                location.Invoke(i.Location) &&
                packaging.Invoke(i.PackagingProduct) &&
                treatment.Invoke(i.Treatment) &&
                i.ToteKey == inventoryKey.InventoryKey_ToteKey);
        }

        internal static Expression<Func<InventoryTransaction, bool>> BySourceLotType(LotTypeEnum lotType)
        {
            return Projector<InventoryTransaction>.To(t => t.SourceLotTypeId == (int) lotType);
        }

        internal static Expression<Func<InventoryTransaction, bool>> ByDateReceivedRangeStart(DateTime date)
        {
            date = date.Date;
            return Projector<InventoryTransaction>.To(t => t.DateCreated >= date);
        }

        internal static Expression<Func<InventoryTransaction, bool>> ByDateReceivedRangeEnd(DateTime date)
        {
            date = date.Date.AddDays(1);
            return Projector<InventoryTransaction>.To(t => t.DateCreated < date);
        }
    }
}