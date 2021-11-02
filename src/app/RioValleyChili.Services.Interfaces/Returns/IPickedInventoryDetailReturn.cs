using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IPickedInventoryDetailReturn : IAttributesByProductType
    {
        string PickedInventoryKey { get; }
        IEnumerable<IPickedInventoryItemReturn> PickedInventoryItems { get; }
    }
}