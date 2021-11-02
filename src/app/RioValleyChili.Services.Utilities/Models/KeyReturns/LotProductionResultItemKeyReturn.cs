using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class LotProductionResultItemKeyReturn : LotKeyReturn, ILotProductionResultItemKey
    {
        internal string ProductionResultItemKey { get { return new LotProductionResultItemKey(this).KeyValue; } }

        public int ProductionResultItemKey_Sequence { get; internal set; }
    }
}