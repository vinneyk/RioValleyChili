using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Utilities.Commands.Parameters;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateMillAndWetdownResultItemCommandParametersExtensions
    {
        internal static ModifyInventoryParameters ToModifyInventoryParameters(this CreateMillAndWetdownResultItemCommandParameters item, ILotKey lotKey)
        {
            return new ModifyInventoryParameters(lotKey, item.PackagingProductKey, item.LocationKey, GlobalKeyHelpers.NoTreatmentKey, " ", item.Quantity);
        }
    }
}