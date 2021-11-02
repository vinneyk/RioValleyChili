using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ChileMaterialsReceivedItemKeyReturn : LotKeyReturn, IChileMaterialsReceivedItemKey
    {
        internal string DehydratedMaterialsReceivedItemKey { get { return new ChileMaterialsReceivedItemKey(this); } }

        public int ChileMaterialsReceivedKey_ItemSequence { get; internal set; }
    }
}