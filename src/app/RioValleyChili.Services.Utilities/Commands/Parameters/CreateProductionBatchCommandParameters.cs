using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateProductionBatchCommandParameters
    {
        public ICreateProductionBatchParameters Parameters { get; set; }

        public PackScheduleKey PackScheduleKey { get; set; }
    }
}