using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateProductionScheduleParameters
    {
        internal ICreateProductionScheduleParameters Parameters { get; set; }
        internal LocationKey ProductionLocationKey { get; set; }
    }
}