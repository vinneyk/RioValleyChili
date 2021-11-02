using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Utilities.Conductors.Parameters
{
    internal class SetPickedInventoryItemCodesParameters
    {
        internal ISetPickedInventoryItemCodesParameters Parameters { get; set; }
        internal PickedInventoryItemKey PickedInventoryItemKey { get; set; }
    }
}