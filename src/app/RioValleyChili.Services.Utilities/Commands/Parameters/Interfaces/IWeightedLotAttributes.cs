using System.Collections.Generic;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    internal interface IWeightedLotAttributes
    {
        double LotWeight { get; }
        List<ILotAttributeParameter> LotAttributes { get; }
    }

    public interface ILotAttributeParameter : IAttributeNameKey
    {
        string Name { get; }
        bool NameActive { get; }
        double Value { get; }
    }
}