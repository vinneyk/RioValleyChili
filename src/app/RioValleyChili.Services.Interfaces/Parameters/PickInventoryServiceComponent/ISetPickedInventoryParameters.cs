using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent
{
    public interface ISetPickedInventoryParameters : IUserIdentifiable
    {
        IEnumerable<IPickedInventoryItemParameters> PickedInventoryItems { get; }
    }
}