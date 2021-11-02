using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryPickOrderKeyReturn : IInventoryPickOrderKey
    {
        internal string InventoryPickOrderKey { get { return new InventoryPickOrderKey(this).KeyValue; } }

        public DateTime InventoryPickOrderKey_DateCreated { get; internal set; }

        public int InventoryPickOrderKey_Sequence { get; internal set; }
    }
}