using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ShipmentInformationKeyReturn : IShipmentInformationKey
    {
        internal string ShipmentInfoKey { get { return new ShipmentInformationKey(this).KeyValue; } }

        public DateTime ShipmentInfoKey_DateCreated { get; internal set; }

        public int ShipmentInfoKey_Sequence { get; internal set; }
    }
}