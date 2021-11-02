using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    public static class ChileLotExtensions
    {
        public static IQueryable<Inventory> FilterByInventoryKeys(this IQueryable<Inventory> inventoryQuery, IEnumerable<IInventoryKey> inventoryKeys)
        {
            if(inventoryQuery == null) { throw new ArgumentNullException("inventoryQuery"); }
            if(inventoryKeys == null) { throw new ArgumentNullException("inventoryKeys"); }
            
            if(inventoryQuery as DbQuery<Inventory> != null)
            {
                var stringBuilder = new InventoryKey();
                var lotNumberStrings = inventoryKeys.ToList().Select(stringBuilder.BuildKeyValue).ToList();
                return inventoryQuery.Where(c => lotNumberStrings.Contains(
                        SqlFunctions.StringConvert((double?)c.LotDateCreated.Year).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotDateCreated.Month).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotDateCreated.Day).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotDateSequence).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotTypeId).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LocationId).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.PackagingProductId).Trim()));
            }

            return inventoryQuery.Where(c => inventoryKeys.Any(n =>
                n.LotKey_DateCreated == c.LotKey_DateCreated &&
                n.LotKey_DateSequence == c.LotKey_DateSequence &&
                n.LotKey_LotTypeId == c.LotKey_LotTypeId &&
                n.LocationKey_Id == c.LocationId &&
                n.PackagingProductKey_ProductId == c.PackagingProductId));
        }

        public static IQueryable<ChileLot> FilterByChileLotNumbers(this IQueryable<ChileLot> chileLotQuery, IEnumerable<ILotKey> lotNumbers)
        {
            if(chileLotQuery == null) { throw new ArgumentNullException("chileLotQuery"); }
            if(lotNumbers == null) { throw new ArgumentNullException("lotNumbers"); }
            
            if(chileLotQuery as DbQuery<ChileLot> != null)
            {
                var lotNumberStrings = lotNumbers.Select(l => string.Format("{0}-{1}-{2}-{3}-{4}", l.LotKey_DateCreated.Year, l.LotKey_DateCreated.Month, l.LotKey_DateCreated.Day, l.LotKey_DateSequence, l.LotKey_LotTypeId)).ToList();
                return chileLotQuery.Where(c => lotNumberStrings.Contains(
                        SqlFunctions.StringConvert((double?)c.LotDateCreated.Year).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotDateCreated.Month).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotDateCreated.Day).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotDateSequence).Trim() + "-" +
                        SqlFunctions.StringConvert((double?)c.LotTypeId).Trim()));
            }

            return chileLotQuery.Where(c => lotNumbers.Any(n =>
                n.LotKey_DateCreated == c.LotDateCreated &&
                n.LotKey_DateSequence == c.LotDateSequence &&
                n.LotKey_LotTypeId == c.LotTypeId));
        }
    }
}
