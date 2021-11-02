using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerContractOrderReturn
    {
        string CustomerOrderKey { get; }

        SalesOrderStatus SalesOrderStatus { get; }

        ShipmentStatus ShipmentStatus { get; }

        IEnumerable<ICustomerContractOrderItemReturn> Items { get; }
    }
}