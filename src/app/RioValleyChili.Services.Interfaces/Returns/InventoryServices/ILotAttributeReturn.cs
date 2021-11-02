using System;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface ILotAttributeReturn
    {
        string Key { get; }
        string Name { get; }
        double Value { get; }
        DateTime AttributeDate { get; }
        bool Computed { get; }
    }
}