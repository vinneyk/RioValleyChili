using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface ILotDefectResolutionReturn
    {
        ResolutionTypeEnum? ResolutionType { get; }

        string Description { get; }
    }
}