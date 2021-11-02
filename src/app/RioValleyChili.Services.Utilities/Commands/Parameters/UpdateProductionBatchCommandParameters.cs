using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateProductionBatchCommandParameters
    {
        public IUpdateProductionBatchParameters Parameters { get; set; }

        public LotKey ProductionBatchKey { get; set; }
    }
}