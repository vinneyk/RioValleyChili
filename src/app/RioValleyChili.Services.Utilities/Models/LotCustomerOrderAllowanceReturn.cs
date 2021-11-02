using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotCustomerOrderAllowanceReturn : ILotCustomerOrderAllowanceReturn
    {
        public string OrderKey { get { return SalesOrderKeyReturn.CustomerOrderKey; } }
        public int? OrderNumber { get; internal set; }
        public string CustomerKey { get { return CustomerKeyReturn.CustomerKey; } }
        public string CustomerName { get; internal set; }

        internal SalesOrderKeyReturn SalesOrderKeyReturn { get; set; }
        internal CustomerKeyReturn CustomerKeyReturn { get; set; }
    }
}