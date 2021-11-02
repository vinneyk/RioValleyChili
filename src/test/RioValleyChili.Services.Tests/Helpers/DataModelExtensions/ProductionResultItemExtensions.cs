using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ProductionResultItemExtensions
    {
        internal static LotProductionResultItem ConstrainByKeys(this LotProductionResultItem item, ILotKey lotKey, IPackagingProductKey packagingProductKey = null)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            item.ProductionResults = null;
            item.LotDateCreated = lotKey.LotKey_DateCreated;
            item.LotDateSequence = lotKey.LotKey_DateSequence;
            item.LotTypeId = lotKey.LotKey_LotTypeId;

            if(packagingProductKey != null)
            {
                item.PackagingProduct = null;
                item.PackagingProductId = packagingProductKey.PackagingProductKey_ProductId;
            }

            return item;
        }

        internal static LotProductionResultItem SetToInventory(this LotProductionResultItem item, IInventoryKey inventoryKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }
            if(inventoryKey == null) { throw new ArgumentNullException("inventoryKey"); }

            item.ProductionResults = null;
            item.LotDateCreated = inventoryKey.LotKey_DateCreated;
            item.LotDateSequence = inventoryKey.LotKey_DateSequence;
            item.LotTypeId = inventoryKey.LotKey_LotTypeId;

            item.Location = null;
            item.LocationId = inventoryKey.LocationKey_Id;

            item.PackagingProduct = null;
            item.PackagingProductId = inventoryKey.PackagingProductKey_ProductId;

            item.Treatment = null;
            item.TreatmentId = inventoryKey.InventoryTreatmentKey_Id;

            return item;
        }

        internal static void AssertEqual(this LotProductionResultItem item, IProductionResultItemReturn itemReturn)
        {
            if(item == null) { throw new ArgumentNullException("item"); }
            if(itemReturn == null) { throw new ArgumentNullException("itemReturn"); }

            Assert.AreEqual(new LotProductionResultItemKey(item).KeyValue, itemReturn.ProductionResultItemKey);
            Assert.AreEqual(item.Quantity, itemReturn.Quantity);
            item.PackagingProduct.AssertEqual(itemReturn.PackagingProduct);
            item.Location.AssertEqual(itemReturn.Location);
            item.Treatment.AssertEqual(itemReturn.Treatment);
        }
    }
}