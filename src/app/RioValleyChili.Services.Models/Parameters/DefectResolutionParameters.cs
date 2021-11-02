using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class DefectResolutionParameters : IDefectResolutionParameters
    {
        public ResolutionTypeEnum ResolutionType { get; set; }
        public string Description { get; set; }
    }
}