using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IInventoryShipmentOrderKey
    {
        DateTime InventoryShipmentOrderKey_DateCreated { get; }
        int InventoryShipmentOrderKey_Sequence { get; }
    }
}