using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class SetInventoryPickOrderParameters : ISetInventoryPickOrderParameters
    {
        public string OrderKey { get; set; }
        public string UserToken { get; set; }
        public IEnumerable<InventoryPickOrderItem> InventoryPickOrderItems { get; set; }

        IEnumerable<ISetInventoryPickOrderItemParameters> ISetInventoryPickOrderParameters.InventoryPickOrderItems { get { return InventoryPickOrderItems; } } 
    }
}