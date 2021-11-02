using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class SalesOrderItemKeyReturn : ISalesOrderItemKey
    {
        internal string SalesOrderKey { get { return this.ToSalesOrderKey(); } }
        internal string SalesOrderItemKey { get { return this.ToSalesOrderItemKey(); } }

        public DateTime SalesOrderKey_DateCreated { get; internal set; }
        public int SalesOrderKey_Sequence { get; internal set; }
        public int SalesOrderItemKey_ItemSequence { get; internal set; }
    }
}