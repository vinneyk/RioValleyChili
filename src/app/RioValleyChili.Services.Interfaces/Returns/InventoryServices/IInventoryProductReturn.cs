using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IInventoryProductReturn
    {
        string ProductKey { get; }
        string ProductName { get; }
        ProductTypeEnum? ProductType { get; }
        string ProductSubType { get; }
        string ProductCode { get; }
        bool IsActive { get; }
    }
}