using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerContractOrderReturn : ICustomerContractOrderReturn
    {
        public string CustomerOrderKey { get { return SalesOrderKeyReturn.CustomerOrderKey; } }

        public SalesOrderStatus SalesOrderStatus { get; internal set; }

        public ShipmentStatus ShipmentStatus { get; internal set; }

        public IEnumerable<ICustomerContractOrderItemReturn> Items { get; internal set; }

        internal SalesOrderKeyReturn SalesOrderKeyReturn { get; set; }
    }
}