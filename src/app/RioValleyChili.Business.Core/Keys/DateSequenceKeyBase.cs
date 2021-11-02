using System;
using System.Globalization;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public abstract class DateSequenceKeyBase<TKey> : EntityKey<TKey>.With<DateTime, int>
        where TKey : class
    {
        protected DateSequenceKeyBase() { }

        protected DateSequenceKeyBase(TKey key) : base(key) { }

        protected sealed override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        protected sealed override bool TryParseDateTime(string s, out object result)
        {
            DateTime dateTime;
            var tryParse = DateTime.TryParseExact(s, "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateTime);
            result = dateTime;
            return tryParse;
        }
    }
}