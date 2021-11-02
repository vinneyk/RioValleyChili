using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class AllowanceParameters : ILotAllowanceParameters
    {
        public string LotKey { get; set; }
        public string ContractKey { get; set; }
        public string CustomerOrderKey { get; set; }
        public string CustomerKey { get; set; }
    }
}