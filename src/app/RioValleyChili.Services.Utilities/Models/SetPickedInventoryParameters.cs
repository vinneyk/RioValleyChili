using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SetPickedInventoryParameters : ISetPickedInventoryParameters
    {
        public string UserToken { get; set; }

        public IEnumerable<IPickedInventoryItemParameters> PickedInventoryItems { get; set; }
    }
}