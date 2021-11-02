using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface ISalesQuoteKey
    {
        DateTime SalesQuoteKey_DateCreated { get; }
        int SalesQuoteKey_Sequence { get; }
    }
}