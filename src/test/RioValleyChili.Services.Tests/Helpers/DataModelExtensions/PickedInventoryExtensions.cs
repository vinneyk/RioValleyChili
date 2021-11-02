using System;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class PickedInventoryExtensions
    {
        internal static PickedInventory EmptyItems(this PickedInventory pickedInventory)
        {
            if(pickedInventory == null) { throw new ArgumentNullException("pickedInventory"); }

            pickedInventory.Items = null;

            return pickedInventory;
        }
    }
}