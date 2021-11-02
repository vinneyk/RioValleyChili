using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface ILotDefectReturn
    {
        string LotDefectKey { get; }

        DefectTypeEnum DefectType { get; }

        string Description { get; }

        ILotAttributeDefectReturn AttributeDefect { get; }

        ILotDefectResolutionReturn Resolution { get; }
    }
}