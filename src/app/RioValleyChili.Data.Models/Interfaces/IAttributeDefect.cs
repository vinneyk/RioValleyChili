using RioValleyChili.Core;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IAttributeDefect : IAttributeRange
    {
        double Value { get; }
        DefectTypeEnum DefectType { get; }
        bool HasResolution { get; }
    }
}