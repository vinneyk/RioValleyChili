using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IInventoryPickOrderKey
    {
        DateTime InventoryPickOrderKey_DateCreated { get; }

        int InventoryPickOrderKey_Sequence { get; }
    }
}