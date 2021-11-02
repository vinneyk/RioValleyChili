using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface IDefectResolutionParameters
    {
        ResolutionTypeEnum ResolutionType { get; }

        string Description { get; }
    }
}