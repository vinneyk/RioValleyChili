using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ScheduledProductionBatchReturn : IScheduledProductionBatchReturn
    {
        public string ProductionBatchKey { get { return OutputLotKeyReturn.LotKey; } }

        public string OutputLotKey { get { return OutputLotKeyReturn.LotKey; } }

        internal LotKeyReturn OutputLotKeyReturn { get; set; }
    }
}