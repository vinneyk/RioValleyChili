using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Enumerations;

namespace RioValleyChili.Services.Utilities.Helpers
{
    public static class ScheduleInventoryPickOrderItemsHelper
    {
        public static List<ISchedulePickOrderItemParameter> ScheduleStatus(InventoryPickOrderKey pickOrderKey, List<InventoryPickOrderItem> existingItems, List<ISchedulePickOrderItemParameter> newItems)
        {
            var scheduledItems = existingItems.Select(i => new InventoryPickOrderItemParameter(i) as ISchedulePickOrderItemParameter).ToList();
            scheduledItems.ForEach(i => i.Status = ScheduledStatus.Remove);

            foreach(var newItem in newItems.Where(i => i.Quantity > 0))
            {
                var candidates = scheduledItems.Where(i => i.Status == ScheduledStatus.Remove).ToList();
                candidates.ForEach(i => { i.MatchedFieldsCount = MatchFields.Count(m => m(i, newItem)); });
                var closestMatch = candidates.OrderByDescending(i => i.MatchedFieldsCount).FirstOrDefault();
                if(closestMatch != null)
                {
                    if(closestMatch.MatchedFieldsCount == MatchFields.Count)
                    {
                        closestMatch.Status = ScheduledStatus.None;
                    }
                    else
                    {
                        UpdateItem(closestMatch, newItem);
                    }
                }
                else
                {
                    newItem.PickOrderKey = pickOrderKey;
                    newItem.Status = ScheduledStatus.Create;
                    scheduledItems.Add(newItem);
                }
            }

            return scheduledItems;
        }

        private static void UpdateItem(ISchedulePickOrderItemParameter item, ISchedulePickOrderItemParameter updateTo)
        {
            item.Status = ScheduledStatus.Update;
            item.ProductId = updateTo.ProductId;
            item.PackagingProductId = updateTo.PackagingProductId;
            item.TreatmentId = updateTo.TreatmentId;
            item.Quantity = updateTo.Quantity;
            item.CustomerLotCode = updateTo.CustomerLotCode;
            item.CustomerProductCode = updateTo.CustomerProductCode;
            item.CustomerKey = updateTo.CustomerKey;
        }

        private static readonly List<Func<ISchedulePickOrderItemParameter, ISchedulePickOrderItemParameter, bool>> MatchFields = new List<Func<ISchedulePickOrderItemParameter, ISchedulePickOrderItemParameter, bool>>
            {
                (a, b) => a.ProductId == b.ProductId,
                (a, b) => a.PackagingProductId == b.PackagingProductId,
                (a, b) => a.TreatmentId == b.TreatmentId,
                (a, b) => a.Quantity == b.Quantity,
                (a, b) => a.CustomerLotCode == b.CustomerLotCode,
                (a, b) => a.CustomerProductCode == b.CustomerProductCode,
                (a, b) => a.CustomerKey != null && b.CustomerKey != null && a.CustomerKey.KeyValue == b.CustomerKey.KeyValue || a.CustomerKey == null && b.CustomerKey == null
            };
    }
}