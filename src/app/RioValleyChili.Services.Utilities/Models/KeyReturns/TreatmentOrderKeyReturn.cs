using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class TreatmentOrderKeyReturn : ITreatmentOrderKey
    {
        internal string TreatmentOrderKey { get { return new TreatmentOrderKey(this).KeyValue; } }

        public DateTime InventoryShipmentOrderKey_DateCreated { get; internal set; }
        public int InventoryShipmentOrderKey_Sequence { get; internal set; }
    }
}