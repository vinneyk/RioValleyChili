using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IPickedInventoryKey
    {
        DateTime PickedInventoryKey_DateCreated { get; }

        int PickedInventoryKey_Sequence { get; }
    }
}