using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryAdjustmentExtensions
    {
        internal static void AssertEqual(this InventoryAdjustment inventoryAdjustment, IInventoryAdjustmentReturn adjustmentReturn, List<dynamic> derivedLots = null)
        {
            if(inventoryAdjustment == null) { throw new ArgumentNullException("inventoryAdjustment"); }
            if(adjustmentReturn == null) { throw new ArgumentNullException("adjustmentReturn"); }

            Assert.AreEqual(new InventoryAdjustmentKey(inventoryAdjustment).KeyValue, adjustmentReturn.InventoryAdjustmentKey);
            Assert.AreEqual(inventoryAdjustment.AdjustmentDate, adjustmentReturn.AdjustmentDate);
            Assert.AreEqual(inventoryAdjustment.Employee.UserName, adjustmentReturn.User);
            Assert.AreEqual(inventoryAdjustment.TimeStamp, adjustmentReturn.TimeStamp);
            inventoryAdjustment.Notebook.AssertEqual(adjustmentReturn.Notebook);

            if(!(inventoryAdjustment.Items == null && adjustmentReturn.Items == null))
            {
                var items = (inventoryAdjustment.Items ?? new InventoryAdjustmentItem[0]).ToList();
                var returnItems = adjustmentReturn.Items.ToList();

                Assert.AreEqual(items.Count, returnItems.Count);
                items.ForEach(i =>
                    {
                        var inventoryAdjustmentItemKey = new InventoryAdjustmentItemKey(i);
                        i.AssertEqual(returnItems.Single(r => inventoryAdjustmentItemKey.KeyValue == r.InventoryAdjustmentItemKey), derivedLots);
                    });
            }
        }
    }
}