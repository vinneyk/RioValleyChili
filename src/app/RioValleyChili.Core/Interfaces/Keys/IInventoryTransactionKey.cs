using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IInventoryTransactionKey
    {
        DateTime InventoryTransactionKey_Date { get; }
        int InventoryTransactionKey_Sequence { get; }
    }
}