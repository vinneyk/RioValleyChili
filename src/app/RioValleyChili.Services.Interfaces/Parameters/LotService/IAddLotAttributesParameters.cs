using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface IAddLotAttributesParameters : IUserIdentifiable
    {
        IEnumerable<string> LotKeys { get; }
        IDictionary<string, IAttributeValueParameters> Attributes { get; }

        bool OverrideOldContextLotAsCompleted { get; }
    }
}