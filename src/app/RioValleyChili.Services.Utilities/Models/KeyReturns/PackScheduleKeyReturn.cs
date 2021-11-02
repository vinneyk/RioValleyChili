using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class PackScheduleKeyReturn : IPackScheduleKey
    {
        internal string PackScheduleKey { get { return new PackScheduleKey(this).KeyValue; } }

        public DateTime PackScheduleKey_DateCreated { get; internal set; }

        public int PackScheduleKey_DateSequence { get; internal set; }
    }
}