using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class SalesQuoteKeyReturn : ISalesQuoteKey
    {
        internal string SalesQuoteKey { get { return this.ToSalesQuoteKey(); } } 
        public DateTime SalesQuoteKey_DateCreated { get; internal set; }
        public int SalesQuoteKey_Sequence { get; internal set; }
    }
}