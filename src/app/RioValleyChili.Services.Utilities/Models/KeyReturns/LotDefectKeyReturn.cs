using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class LotDefectKeyReturn : ILotDefectKey
    {
        internal string LotDefectKey { get { return new LotDefectKey(this).KeyValue; } }

        public DateTime LotKey_DateCreated { get; internal set; }

        public int LotKey_DateSequence { get; internal set; }

        public int LotKey_LotTypeId { get; internal set; }

        public int LotDefectKey_Id { get; internal set; }
    }
}