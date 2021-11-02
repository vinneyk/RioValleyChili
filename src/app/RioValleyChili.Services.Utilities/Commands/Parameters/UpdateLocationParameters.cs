using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateLocationParameters
    {
        public IUpdateLocationParameters Params;
        public LocationKey LocationKey;
    }
}