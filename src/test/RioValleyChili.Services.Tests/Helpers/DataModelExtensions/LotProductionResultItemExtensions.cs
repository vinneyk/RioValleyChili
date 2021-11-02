using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotProductionResultItemExtensions
    {
        internal static LotProductionResultItem Set(this LotProductionResultItem item, ILotKey lotKey, IPackagingProductKey packagingKey = null)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(lotKey != null)
            {
                item.ProductionResults = null;
                item.LotDateCreated = lotKey.LotKey_DateCreated;
                item.LotDateSequence = lotKey.LotKey_DateSequence;
                item.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(packagingKey != null)
            {
                item.PackagingProduct = null;
                item.PackagingProductId = packagingKey.PackagingProductKey_ProductId;
            }

            return item;
        }
    }
}