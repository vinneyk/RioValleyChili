using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ISetLotStatusParameters : IUserIdentifiable
    {
        string LotKey { get; }
        LotQualityStatus QualityStatus { get; }
    }
}