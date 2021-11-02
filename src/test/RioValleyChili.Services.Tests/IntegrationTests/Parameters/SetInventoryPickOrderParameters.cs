using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetInventoryPickOrderParameters : ISetInventoryPickOrderParameters
    {
        public SetInventoryPickOrderParameters(string orderKey)
        {
            OrderKey = orderKey;
        }

        public string OrderKey { get; set; }
        public string UserToken { get; set; }

        public IEnumerable<ISetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; set; }
    }
}
