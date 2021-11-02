using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ResolutionsByDefectTypeReturn : IResolutionsByDefectTypeReturn
    {
        public IEnumerable<KeyValuePair<DefectTypeEnum, IEnumerable<ResolutionTypeEnum>>> DefectResolutions { get; internal set; }
    }
}