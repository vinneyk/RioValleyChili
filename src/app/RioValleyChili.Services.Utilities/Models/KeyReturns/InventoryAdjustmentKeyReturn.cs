using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryAdjustmentKeyReturn : IInventoryAdjustmentKey
    {
        internal string InventoryAdjustmentKey { get { return new InventoryAdjustmentKey(this).KeyValue; } }

        public DateTime InventoryAdjustmentKey_AdjustmentDate { get; internal set; }

        public int InventoryAdjustmentKey_Sequence { get; internal set; }
    }
}