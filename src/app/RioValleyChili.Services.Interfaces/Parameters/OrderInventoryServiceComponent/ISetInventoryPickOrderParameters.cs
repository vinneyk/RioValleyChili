using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent
{
    public interface ISetInventoryPickOrderParameters
    {
        string OrderKey { get; }

        string UserToken { get; }

        IEnumerable<ISetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; }
    }
}