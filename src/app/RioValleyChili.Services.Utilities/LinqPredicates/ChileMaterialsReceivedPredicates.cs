using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ChileMaterialsReceivedPredicates
    {
        internal static Expression<Func<ChileMaterialsReceived, bool>> BySupplierKey(IKey<Company> supplierKey)
        {
            var supplierPredicate = supplierKey.FindByPredicate;
            return m => supplierPredicate.Invoke(m.Supplier);
        }

        public static Expression<Func<ChileMaterialsReceived, bool>> ByChileProductKey(IKey<ChileProduct> chileProductKey)
        {
            var chileProductPredicate = chileProductKey.FindByPredicate;
            return m => chileProductPredicate.Invoke(m.ChileLot.ChileProduct);
        }

        public static Expression<Func<ChileMaterialsReceived, bool>> ByChileMaterialsType(ChileMaterialsReceivedType? chileMaterialsType)
        {
            return m => m.ChileMaterialsReceivedType == chileMaterialsType;
        }
    }
}