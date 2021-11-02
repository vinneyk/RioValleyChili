using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class ResolveLotDefectParameters : IResolveLotDefectParameters
    {
        public string UserToken { get; set; }

        public ResolutionTypeEnum ResolutionType { get; set; }

        public string Description { get; set; }

        public string LotDefectKey { get; set; }
    }
}