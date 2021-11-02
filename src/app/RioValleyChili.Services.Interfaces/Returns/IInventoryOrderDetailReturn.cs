using System;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IInventoryOrderDetailReturn<out TPickOrder, out TPickOrderItem>
        where TPickOrder : IPickOrderDetailReturn<TPickOrderItem>
        where TPickOrderItem : IPickOrderItemReturn
    {
        string MovementKey { get; }
        DateTime DateCreated { get; }
        InventoryOrderEnum InventoryOrderEnum { get; }

        TPickOrder PickOrder { get; }
        IPickedInventoryDetailReturn PickedInventory { get; }
    }
}