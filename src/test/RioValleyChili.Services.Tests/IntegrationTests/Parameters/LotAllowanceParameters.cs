using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class LotAllowanceParameters : ILotAllowanceParameters
    {
        public string LotKey { get; set; }
        public string ContractKey { get; set; }
        public string CustomerOrderKey { get; set; }
        public string CustomerKey { get; set; }
    }
}