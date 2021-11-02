using System;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ProductionBatchPredicates
    {
        internal static Expression<Func<ProductionBatch, bool>> ByLotKey(ILotKey lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            return b => b.LotDateCreated == lotKey.LotKey_DateCreated && b.LotDateSequence == lotKey.LotKey_DateSequence && b.LotTypeId == lotKey.LotKey_LotTypeId;
        }

        internal static Expression<Func<DateTime, ProductionBatch, bool>> ForProduction()
        {
            return (d, b) => b.LotDateCreated <= d && !(new[] { b.Production.Results }.Any(r => r.ProductionEnd < d));
        }

        internal static Expression<Func<TLotKeyEntity, ProductionBatch, bool>> ByLotKeyEntity<TLotKeyEntity>()
            where TLotKeyEntity : LotKeyEntityBase
        {
            return (e, b) => b.LotTypeId == e.LotTypeId && b.LotDateCreated == e.LotDateCreated && b.LotDateSequence == e.LotDateSequence;
        }
    }
}