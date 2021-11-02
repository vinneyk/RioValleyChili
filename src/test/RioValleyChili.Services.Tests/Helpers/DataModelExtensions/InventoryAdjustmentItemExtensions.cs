using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryAdjustmentItemExtensions
    {
        internal static InventoryAdjustmentItem ConstrainByKeys(this InventoryAdjustmentItem item, IInventoryAdjustmentKey adjustmentKey, ILotKey lotKey = null)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(adjustmentKey != null)
            {
                item.InventoryAdjustment = null;
                item.AdjustmentDate = adjustmentKey.InventoryAdjustmentKey_AdjustmentDate;
                item.Sequence = adjustmentKey.InventoryAdjustmentKey_Sequence;
            }

            if(lotKey != null)
            {
                item.Lot = null;
                item.LotDateCreated = lotKey.LotKey_DateCreated;
                item.LotDateSequence = lotKey.LotKey_DateSequence;
                item.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            return item;
        }

        internal static void AssertEqual(this InventoryAdjustmentItem inventoryAdjustmentItem, IInventoryAdjustmentItemReturn inventoryAdjustmentItemReturn, List<dynamic> derivedLots = null)
        {
            if(inventoryAdjustmentItem == null) { throw new ArgumentNullException("inventoryAdjustmentItem"); }
            if(inventoryAdjustmentItemReturn == null) { throw new ArgumentNullException("inventoryAdjustmentItemReturn"); }

            Assert.AreEqual(new InventoryAdjustmentItemKey(inventoryAdjustmentItem).KeyValue, inventoryAdjustmentItemReturn.InventoryAdjustmentItemKey);
            Assert.AreEqual(inventoryAdjustmentItem.QuantityAdjustment, inventoryAdjustmentItemReturn.AdjustmentQuantity);

            var lotKey = new LotKey(inventoryAdjustmentItem);
            Assert.AreEqual(lotKey.KeyValue, inventoryAdjustmentItemReturn.LotKey);
            Assert.AreEqual(inventoryAdjustmentItem.ToteKey, inventoryAdjustmentItemReturn.ToteKey);

            inventoryAdjustmentItem.Location.AssertEqual(inventoryAdjustmentItemReturn.Location);
            inventoryAdjustmentItem.PackagingProduct.AssertEqual(inventoryAdjustmentItemReturn.PackagingProduct);
            inventoryAdjustmentItem.Treatment.AssertEqual(inventoryAdjustmentItemReturn.InventoryTreatment);

            if(inventoryAdjustmentItem.Lot.ChileLot != null)
            {
                inventoryAdjustmentItem.Lot.ChileLot.ChileProduct.AssertEqual(inventoryAdjustmentItemReturn.InventoryProduct);
            }
            else if(inventoryAdjustmentItem.Lot.AdditiveLot != null)
            {
                inventoryAdjustmentItem.Lot.AdditiveLot.AdditiveProduct.AssertEqual(inventoryAdjustmentItemReturn.InventoryProduct);
            }
            else if(inventoryAdjustmentItem.Lot.PackagingLot != null)
            {
                inventoryAdjustmentItem.Lot.PackagingLot.PackagingProduct.AssertEqual(inventoryAdjustmentItemReturn.InventoryProduct);
            }
            else
            {
                Assert.IsNull(inventoryAdjustmentItemReturn.InventoryProduct);
            }
        }
    }
}