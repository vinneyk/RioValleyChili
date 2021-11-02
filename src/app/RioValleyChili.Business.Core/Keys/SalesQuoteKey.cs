using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class SalesQuoteKey : DateSequenceKeyBase<ISalesQuoteKey>, IKey<SalesQuote>, ISalesQuoteKey
    {
        public SalesQuoteKey() { }

        public SalesQuoteKey(ISalesQuoteKey salesQuoteKey) : base(salesQuoteKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidSalesQuoteKey, inputValue);
        }

        protected override ISalesQuoteKey ConstructKey(DateTime field0, int field1)
        {
            return new SalesQuoteKey
                {
                    SalesQuoteKey_DateCreated = field0,
                    SalesQuoteKey_Sequence = field1
                };
        }

        protected override With<DateTime, int> DeconstructKey(ISalesQuoteKey key)
        {
            return new SalesQuoteKey
                {
                    SalesQuoteKey_DateCreated = key.SalesQuoteKey_DateCreated,
                    SalesQuoteKey_Sequence = key.SalesQuoteKey_Sequence
                };
        }

        public Expression<Func<SalesQuote, bool>> FindByPredicate { get { return (q => q.DateCreated == Field0 && q.Sequence == Field1); } }
        public DateTime SalesQuoteKey_DateCreated { get { return Field0; } private set { Field0 = value; } }
        public int SalesQuoteKey_Sequence { get { return Field1; } private set { Field1 = value; } }
    }

    public static class SalesQuoteKeyExtensions
    {
        public static SalesQuoteKey ToSalesQuoteKey(this ISalesQuoteKey k)
        {
            return new SalesQuoteKey(k);
        }
    }
}