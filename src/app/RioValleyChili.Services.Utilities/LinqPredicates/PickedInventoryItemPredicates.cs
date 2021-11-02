using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    public static class PickedInventoryItemPredicates
    {
        public static Expression<Func<PickedInventoryItem, bool>> FilterByPickedInventoryKey(IPickedInventoryKey pickedInventoryKey)
        {
            return i => i.DateCreated == pickedInventoryKey.PickedInventoryKey_DateCreated && i.Sequence == pickedInventoryKey.PickedInventoryKey_Sequence;
        }

        public static Expression<Func<PickedInventoryItem, bool>> FilterByLotKey(ILotKey lotKey)
        {
            return i => i.LotDateCreated == lotKey.LotKey_DateCreated && i.LotDateSequence == lotKey.LotKey_DateSequence && i.LotTypeId == lotKey.LotKey_LotTypeId;
        }
    }
}