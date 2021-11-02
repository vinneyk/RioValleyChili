// ReSharper disable RedundantExtendsListEntry

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface IInventoryKey : ILotKey, ILocationKey, IInventoryTreatmentKey, IPackagingProductKey
    {
        string InventoryKey_ToteKey { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry