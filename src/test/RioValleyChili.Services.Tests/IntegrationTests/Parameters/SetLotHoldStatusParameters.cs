using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetLotHoldStatusParameters : ISetLotHoldStatusParameters
    {
        public string UserToken { get; set; }

        public string LotKey { get; set; }

        public ILotHold Hold { get; set; }
    }

    public class LotHold : ILotHold
    {
        public LotHoldType HoldType { get; set; }

        public string Description { get; set; }
    }
}