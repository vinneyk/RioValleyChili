using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotBaseReturn : ILotBaseReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public LotHoldType? HoldType { get; internal set; }
        public string HoldDescription { get; internal set; }
        public LotQualityStatus QualityStatus { get; internal set; }
        public LotProductionStatus ProductionStatus { get; internal set; }
        public string Notes { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }
}