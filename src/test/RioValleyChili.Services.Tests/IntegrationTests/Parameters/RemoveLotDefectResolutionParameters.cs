using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class RemoveLotDefectResolutionParameters : IRemoveLotDefectResolutionParameters
    {
        public string UserToken { get; set; }

        public string LotDefectKey { get; set; }
    }
}