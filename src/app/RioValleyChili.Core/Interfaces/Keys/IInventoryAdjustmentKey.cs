using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IInventoryAdjustmentKey
    {
        DateTime InventoryAdjustmentKey_AdjustmentDate { get; }

        int InventoryAdjustmentKey_Sequence { get; }
    }
}