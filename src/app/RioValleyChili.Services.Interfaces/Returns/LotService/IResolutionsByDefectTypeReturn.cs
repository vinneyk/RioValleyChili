using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface IResolutionsByDefectTypeReturn
    {
        IEnumerable<KeyValuePair<DefectTypeEnum, IEnumerable<ResolutionTypeEnum>>> DefectResolutions { get; }
    }
}