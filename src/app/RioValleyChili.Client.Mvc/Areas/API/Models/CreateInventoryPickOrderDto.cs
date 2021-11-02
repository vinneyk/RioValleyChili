using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateInventoryPickOrderDto : ISetInventoryPickOrderParameters
    {
        public IEnumerable<InventoryPickOrderItem> InventoryPickOrderItems { get; set; }

        public string UserToken { get; set; }

        IEnumerable<ISetInventoryPickOrderItemParameters> ISetInventoryPickOrderParameters.InventoryPickOrderItems
        {
            get { return InventoryPickOrderItems; }
        }

        string ISetInventoryPickOrderParameters.OrderKey
        {
            get { return null; }
        }
    }
}