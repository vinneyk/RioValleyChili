using System;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Helpers;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class PackScheduleKey : EntityKeyBase.Of<IPackScheduleKey>, IKey<PackSchedule>, IPackScheduleKey
    {
        #region fields and constructors
        
        private readonly DateTime _dateCreated;
        private readonly int _dateSequence;

        public PackScheduleKey() { }

        public PackScheduleKey(IPackScheduleKey packScheduleKey)
            : this(packScheduleKey.PackScheduleKey_DateCreated, packScheduleKey.PackScheduleKey_DateSequence) { }

        private PackScheduleKey(DateTime dateCreated, int dateSequence)
        {
            _dateCreated = dateCreated;
            _dateSequence = dateSequence;
        }

        #endregion

        #region Overrides of EntityKeyBase.Of<IPackScheduleKey>

        protected override IPackScheduleKey ParseImplementation(string keyValue)
        {
            if(keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }

            var splitStrings = keyValue.Split('-');
            if(splitStrings.Count() != 2 || splitStrings[0].Length != 7)
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected yyyyddd-#.", keyValue));
            }

            var dateCreated = new DateTime(StringHelper.GetYearFromString(splitStrings[0].Substring(0, 4)), 1, 1);
            dateCreated = dateCreated.AddDays(int.Parse(splitStrings[0].Substring(4, 3)) - 1);

            var dateSequence = int.Parse(splitStrings[1]);

            return new PackScheduleKey(dateCreated, dateSequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidPackScheduleNumber, inputValue);
        }

        public override IPackScheduleKey Default
        {
            get { return Null; }
        }

        protected override IPackScheduleKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IPackScheduleKey key)
        {
            return string.Format("{0}{1:000}-{2}", key.PackScheduleKey_DateCreated.Year, key.PackScheduleKey_DateCreated.DayOfYear, key.PackScheduleKey_DateSequence);
        }

        #endregion

        #region Implementation of IKey<PackSchedule>
        
        public Expression<Func<PackSchedule, bool>> FindByPredicate
        {
            get { return (i => i.DateCreated == _dateCreated && i.SequentialNumber == _dateSequence); }
        }

        #endregion

        #region Implementation of IPackScheduleKey

        public DateTime PackScheduleKey_DateCreated
        {
            get { return _dateCreated; }
        }
        
        public int PackScheduleKey_DateSequence
        {
            get { return _dateSequence; }
        }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IPackScheduleKey != null && Equals(obj as IPackScheduleKey);
        }

        protected bool Equals(IPackScheduleKey other)
        {
            return _dateCreated.Equals(other.PackScheduleKey_DateCreated) && _dateSequence == other.PackScheduleKey_DateSequence;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_dateCreated.GetHashCode() * 397) ^ _dateSequence;
            }
        }

        #endregion

        public static IPackScheduleKey Null = new NullPackScheduleKey();

        private class NullPackScheduleKey : IPackScheduleKey
        {
            #region Implementation of IPackScheduleKey

            public DateTime PackScheduleKey_DateCreated { get { return DateTime.MinValue; } }
            public int PackScheduleKey_DateSequence { get { return 0; } }

            #endregion
        }
    }

    public static class IPackScheduleKeyExtensions
    {
        public static PackScheduleKey ToPackScheduleKey(this IPackScheduleKey k)
        {
            return new PackScheduleKey(k);
        }
    }
}