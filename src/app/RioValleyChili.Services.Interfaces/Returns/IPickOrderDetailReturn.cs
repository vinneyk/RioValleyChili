using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IPickOrderDetailReturn<out TItem>
        where TItem : IPickOrderItemReturn
    {
        string InventoryPickKey { get; }
        IEnumerable<TItem> PickOrderItems { get; }
    }
}