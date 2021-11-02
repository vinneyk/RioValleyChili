using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class IntraWarehouseOrderKeyReturn : IIntraWarehouseOrderKey
    {
        internal string IntraWarehouserOrderKey { get { return new IntraWarehouseOrderKey(this).KeyValue; } }

        public DateTime IntraWarehouseOrderKey_DateCreated { get; internal set; }

        public int IntraWarehouseOrderKey_Sequence { get; internal set; }
    }
}