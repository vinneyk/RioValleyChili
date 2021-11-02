using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class PickedInventoryKeyReturn : IPickedInventoryKey
    {
        internal string PickedInventoryKey { get { return new PickedInventoryKey(this).KeyValue; } }

        public DateTime PickedInventoryKey_DateCreated { get; internal set; }

        public int PickedInventoryKey_Sequence { get; internal set; }
    }
}