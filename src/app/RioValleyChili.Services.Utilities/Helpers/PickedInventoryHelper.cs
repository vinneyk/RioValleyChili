using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal static class PickedInventoryHelper
    {
        internal static List<ModifyPickedInventoryItemParameters> CreateModifyPickedInventoryItemParameters(PickedInventory pickedInventory, List<PickedInventoryParameters> newItems)
        {
            if(pickedInventory.Items == null) { throw new ArgumentNullException("pickedInventory.Items"); }
            var removeModifications = pickedInventory.Items.Select(i => new ModifyPickedInventoryItemParameters(i)).ToList();
            var modifications = new List<ModifyPickedInventoryItemParameters>();

            foreach(var newItem in newItems ?? new List<PickedInventoryParameters>())
            {
                var closestMatch = ClosestMatch(removeModifications, newItem);
                if(closestMatch != null)
                {
                    if(closestMatch.OriginalQuantity != newItem.Quantity ||
                        closestMatch.CustomerLotCode != newItem.CustomerLotCode || closestMatch.CustomerProductCode != newItem.CustomerProductCode ||
                        !closestMatch.CurrentLocationKey.Equals(newItem.CurrentLocationKey))
                    {
                        closestMatch.NewQuantity = newItem.Quantity;
                        closestMatch.CurrentLocationKey = newItem.CurrentLocationKey;
                        closestMatch.CustomerLotCode = newItem.CustomerLotCode;
                        closestMatch.CustomerProductCode = newItem.CustomerProductCode;
                        modifications.Add(closestMatch);
                    }
                    removeModifications.Remove(closestMatch);
                }
                else
                {
                    modifications.Add(new ModifyPickedInventoryItemParameters(newItem.InventoryKey, newItem.CurrentLocationKey, newItem.Quantity, newItem.CustomerLotCode, newItem.CustomerProductCode, newItem.OrderItemKey));
                }
            }

            modifications.AddRange(removeModifications);

            return modifications;
        }

        private static ModifyPickedInventoryItemParameters ClosestMatch(IEnumerable<ModifyPickedInventoryItemParameters> existingItems, PickedInventoryParameters newItem)
        {
            if(existingItems == null) { throw new ArgumentNullException("existingItems"); }
            if(newItem == null) { throw new ArgumentNullException("newItem"); }

            var topMatch = newItem.RankMatches(existingItems.Where(i => i.InventoryKey.Equals(newItem.InventoryKey)),
                    (n, e) =>
                        {
                            if(n.InventoryKey == null && e.InventoryKey == null) { return true; }
                            return n.InventoryKey != null && n.InventoryKey.Equals(e.InventoryKey);
                        },
                    (n, e) =>
                        {
                            if(n.CurrentLocationKey == null && e.CurrentLocationKey == null) { return true; }
                            return n.CurrentLocationKey != null && n.CurrentLocationKey.Equals(e.CurrentLocationKey);
                        },
                    (n, e) => n.Quantity == e.OriginalQuantity,
                    (n, e) => n.CustomerLotCode == e.CustomerLotCode,
                    (n, e) => n.CustomerProductCode == e.CustomerProductCode)
                .FirstOrDefault();

            return topMatch == null ? null : topMatch.Match;
        }

        internal static List<ModifySalesOrderPickedInventoryItemParameters> CreateModifyCustomerOrderPickedInventoryItemParameters(SalesOrder salesOrder, List<PickedInventoryParameters> newItems)
        {
            if(salesOrder == null) { throw new ArgumentNullException("salesOrder"); }
            if(newItems == null) { throw new ArgumentNullException("newItems"); }
            
            var existingManifest = salesOrder.SalesOrderPickedItems.Select(i => new ModifySalesOrderPickedInventoryItemParameters(i)).ToList();
            var newManifest = new List<ModifySalesOrderPickedInventoryItemParameters>();

            foreach(var newItem in newItems)
            {
                var closestMatch = ClosestMatch(existingManifest, newItem);
                if(closestMatch != null)
                {
                    if(closestMatch.OriginalQuantity != newItem.Quantity || closestMatch.SalesOrderItemKey != newItem.OrderItemKey ||
                        closestMatch.CustomerLotCode != newItem.CustomerLotCode || closestMatch.CustomerProductCode != newItem.CustomerProductCode ||
                        !closestMatch.SalesOrderItemKey.Equals(newItem.OrderItemKey))
                    {
                        closestMatch.NewQuantity = newItem.Quantity;
                        closestMatch.SalesOrderItemKey = new SalesOrderItemKey(newItem.OrderItemKey);
                        closestMatch.CustomerLotCode = newItem.CustomerLotCode;
                        closestMatch.CustomerProductCode = newItem.CustomerProductCode;
                        newManifest.Add(closestMatch);
                    }
                    existingManifest.Remove(closestMatch);
                }
                else
                {
                    newManifest.Add(new ModifySalesOrderPickedInventoryItemParameters(newItem.OrderItemKey, newItem.InventoryKey, newItem.Quantity, newItem.CustomerLotCode, newItem.CustomerProductCode));
                }
            }

            newManifest.AddRange(existingManifest);

            return newManifest;
        }

        private static ModifySalesOrderPickedInventoryItemParameters ClosestMatch(IEnumerable<ModifySalesOrderPickedInventoryItemParameters> existingItems, PickedInventoryParameters newItem)
        {
            if(existingItems == null) { throw new ArgumentNullException("existingItems"); }
            if(newItem == null) { throw new ArgumentNullException("newItem"); }

            var sameInventory = existingItems.Where(i => i.InventoryKey.Equals(newItem.InventoryKey)).ToList();
            var topMatch = newItem.RankMatches(sameInventory,
                (n, e) =>
                    {
                        if(n.OrderItemKey == null && e.SalesOrderItemKey == null) { return true; }
                        return n.OrderItemKey != null && n.OrderItemKey.Equals(e.SalesOrderItemKey);
                    },
                (n, e) => n.CustomerLotCode == e.CustomerLotCode,
                (n, e) => n.CustomerProductCode == e.CustomerProductCode,
                (n, e) => n.Quantity == e.OriginalQuantity).FirstOrDefault();

            if(topMatch == null)
            {
                return null;
            }
            return topMatch.Match;
        }
    }
}