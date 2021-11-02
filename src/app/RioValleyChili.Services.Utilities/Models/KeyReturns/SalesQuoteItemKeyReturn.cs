using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class SalesQuoteItemKeyReturn : ISalesQuoteItemKey
    {
        internal string SalesQuoteItemKey { get { return this.ToSalesQuoteItemKey(); } }
        public DateTime SalesQuoteKey_DateCreated { get; internal set; }
        public int SalesQuoteKey_Sequence { get; internal set; }
        public int SalesQuoteItemKey_ItemSequence { get; internal set; }
    }
}