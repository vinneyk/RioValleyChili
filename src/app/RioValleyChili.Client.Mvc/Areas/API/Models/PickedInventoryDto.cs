using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class PickedInventoryDto
    {
        public IEnumerable<PickedInventoryItemDto> PickedInventoryItems { get; set; }    
    }

    internal class SetPickedInventoryParameters : ISetPickedInventoryParameters
    {
        public IEnumerable<SetPickedInventoryItemParameters> PickedInventoryItems { get; set; }
        IEnumerable<IPickedInventoryItemParameters> ISetPickedInventoryParameters.PickedInventoryItems { get { return PickedInventoryItems; } }
        string IUserIdentifiable.UserToken { get; set; }
    }
}