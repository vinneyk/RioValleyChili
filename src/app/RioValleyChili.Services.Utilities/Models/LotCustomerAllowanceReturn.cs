using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotCustomerAllowanceReturn : ILotCustomerAllowanceReturn
    {
        public string CustomerKey { get { return CustomerKeyReturn.CustomerKey; } }
        public string CustomerName { get; internal set; }

        internal CustomerKeyReturn CustomerKeyReturn { get; set; }
    }
}