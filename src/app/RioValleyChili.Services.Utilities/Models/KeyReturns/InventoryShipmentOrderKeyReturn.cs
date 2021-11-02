using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryShipmentOrderKeyReturn : IInventoryShipmentOrderKey
    {
        internal string InventoryShipmentOrderKey { get { return new InventoryShipmentOrderKey(this); } }

        public DateTime InventoryShipmentOrderKey_DateCreated { get; internal set; }
        public int InventoryShipmentOrderKey_Sequence { get; internal set; }
    }
}