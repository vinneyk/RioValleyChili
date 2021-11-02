using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryPickOrderItemKeyReturn : IInventoryPickOrderItemKey
    {
        internal string InventoryPickOrderItemKey { get { return new InventoryPickOrderItemKey(this); } }

        public DateTime InventoryPickOrderKey_DateCreated { get; internal set; }
        public int InventoryPickOrderKey_Sequence { get; internal set; }
        public int InventoryPickOrderItemKey_Sequence { get; internal set; }
    }
}