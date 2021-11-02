using System;

namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IInventoryOrderSummaryReturn
    {
        string MovementKey { get; }
        DateTime DateCreated { get; }
        IInventoryPickOrderSummaryReturn PickOrder { get; }
        IPickedInventorySummaryReturn PickedInventory { get; }
    }
}