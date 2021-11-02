using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetLotPackagingReceivedParameters : ISetLotPackagingReceivedParameters
    {
        public string LotKey { get; set; }
        public string ReceivedPackagingProductKey { get; set; }
    }
}