using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ISetLotAttributeParameters : IUserIdentifiable
    {
        string LotKey { get; }
        string Notes { get; }
        IDictionary<string, IAttributeValueParameters> Attributes { get; }
        
        bool OverrideOldContextLotAsCompleted { get; }
    }
}