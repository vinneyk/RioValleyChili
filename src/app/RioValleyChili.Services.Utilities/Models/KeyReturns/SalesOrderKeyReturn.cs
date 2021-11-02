using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class SalesOrderKeyReturn : ISalesOrderKey
    {
        internal string CustomerOrderKey { get { return new SalesOrderKey(this); } }

        public DateTime SalesOrderKey_DateCreated { get; internal set; }
        public int SalesOrderKey_Sequence { get; internal set; }
    }
}