using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ChileTypeKeyReturn : IChileTypeKey
    {
        internal string ChileTypeKey { get { return new ChileTypeKey(this).KeyValue; } }

        public int ChileTypeKey_ChileTypeId { get; internal set; }
    }
}