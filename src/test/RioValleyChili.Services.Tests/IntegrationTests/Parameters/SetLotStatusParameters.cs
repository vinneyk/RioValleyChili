using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetLotStatusParameters : ISetLotStatusParameters
    {
        public string UserToken { get; set; }

        public string LotKey { get; set; }

        public LotQualityStatus QualityStatus { get; set; }
    }
}