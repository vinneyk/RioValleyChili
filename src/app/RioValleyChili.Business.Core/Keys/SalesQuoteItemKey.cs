using System;
using System.Globalization;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class SalesQuoteItemKey : EntityKey<ISalesQuoteItemKey>.With<DateTime, int, int>, IKey<SalesQuoteItem>, ISalesQuoteItemKey
    {
        public SalesQuoteItemKey() { }

        public SalesQuoteItemKey(ISalesQuoteItemKey salesQuoteItemKey) : base(salesQuoteItemKey.SalesQuoteKey_DateCreated, salesQuoteItemKey.SalesQuoteKey_Sequence, salesQuoteItemKey.SalesQuoteItemKey_ItemSequence) { }

        public override string GetParseFailMessage(string inputValue)
        {
            return string.Format(UserMessages.InvalidSalesQuoteItemKey, inputValue);
        }

        protected override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        protected override bool TryParseDateTime(string s, out object result)
        {
            DateTime dateTime;
            var tryParse = DateTime.TryParseExact(s, "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateTime);
            result = dateTime;
            return tryParse;
        }

        protected override ISalesQuoteItemKey ConstructKey(DateTime field0, int field1, int field2)
        {
            return new SalesQuoteItemKey { Field0 = field0, Field1 = field1, Field2 = field2 };
        }

        protected override With<DateTime, int, int> DeconstructKey(ISalesQuoteItemKey key)
        {
            return new SalesQuoteItemKey { Field0 = key.SalesQuoteKey_DateCreated, Field1 = key.SalesQuoteKey_Sequence, Field2 = key.SalesQuoteItemKey_ItemSequence };
        }

        public Expression<Func<SalesQuoteItem, bool>> FindByPredicate
        {
            get { return i => i.DateCreated == Field0 && i.Sequence == Field1 && i.ItemSequence == Field2; }
        }

        public DateTime SalesQuoteKey_DateCreated { get { return Field0; } }
        public int SalesQuoteKey_Sequence { get { return Field1; } }
        public int SalesQuoteItemKey_ItemSequence { get { return Field2; } }
    }

    public static class SalesQuoteItemkeyExtensions
    {
        public static SalesQuoteItemKey ToSalesQuoteItemKey(this ISalesQuoteItemKey k)
        {
            return new SalesQuoteItemKey(k);
        }
    }
}