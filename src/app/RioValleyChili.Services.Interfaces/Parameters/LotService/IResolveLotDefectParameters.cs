using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface IResolveLotDefectParameters : IDefectResolutionParameters, IUserIdentifiable
    {
        string LotDefectKey { get; }
    }

    public interface IRemoveLotDefectResolutionParameters : IUserIdentifiable
    {
        string LotDefectKey { get; }
    }
}