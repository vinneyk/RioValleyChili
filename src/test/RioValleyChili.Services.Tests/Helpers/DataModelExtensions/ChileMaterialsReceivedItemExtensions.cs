using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ChileMaterialsReceivedItemExtensions
    {
        internal static ChileMaterialsReceivedItem ConstrainByKeys(this ChileMaterialsReceivedItem item, ILotKey lotKey = null)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(lotKey != null)
            {
                item.ChileMaterialsReceived = null;
                item.LotDateCreated = lotKey.LotKey_DateCreated;
                item.LotDateSequence = lotKey.LotKey_DateSequence;
                item.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            return item;
        }

        internal static Inventory ToInventory(this ChileMaterialsReceivedItem item, IInventoryTreatmentKey treatment)
        {
            return new Inventory
                {
                    Quantity = item.Quantity
                }.ConstrainByKeys(item.ChileMaterialsReceived, item, item, treatment, null, item.ToteKey);
        }
    }
}