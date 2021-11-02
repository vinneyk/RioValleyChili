using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IAttributeRange : IAttributeNameKey
    {
        double RangeMin { get; }
        double RangeMax { get; }
    }
}