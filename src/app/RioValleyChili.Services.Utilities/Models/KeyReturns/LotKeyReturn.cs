using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class LotKeyReturn : ILotKey
    {
        internal string LotKey { get { return new LotKey(this).KeyValue; } }

        public int LotKey_LotTypeId { get; internal set; }
        public DateTime LotKey_DateCreated { get; internal set; }
        public int LotKey_DateSequence { get; internal set; }
    }
}