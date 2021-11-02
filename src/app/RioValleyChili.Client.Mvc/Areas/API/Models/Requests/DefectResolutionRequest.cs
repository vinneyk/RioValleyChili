using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class DefectResolutionRequest
    {
        public ResolutionTypeEnum ResolutionType { get; set; }
        public string Description { get; set; }
    }
}