using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryAdjustmentItemKeyReturn : IInventoryAdjustmentItemKey
    {
        internal string InventoryAdjustmentItemKey { get { return new InventoryAdjustmentItemKey(this).KeyValue; } }

        public DateTime InventoryAdjustmentKey_AdjustmentDate { get; internal set; }

        public int InventoryAdjustmentKey_Sequence { get; internal set; }

        public int InventoryAdjustmetItemKey_Sequence { get; internal set; }
    }
}