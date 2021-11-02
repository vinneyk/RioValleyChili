using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class LotAllowanceParameters
    {
        internal LotKey LotKey { get; set; }
        internal CustomerKey CustomerKey { get; set; }
        internal ContractKey ContractKey { get; set; }
        internal SalesOrderKey SalesOrderKey { get; set; }
    }
}