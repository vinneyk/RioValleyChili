using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class IntraWarehouseOrderKey : EntityKeyBase.Of<IIntraWarehouseOrderKey>, IKey<IntraWarehouseOrder>, IIntraWarehouseOrderKey
    {
        #region Fields and Constructors.

        private const string SEPARATOR = "-";

        private readonly DateTime _dateCreated;
        private readonly int _sequence;

        public IntraWarehouseOrderKey() : this(Null) { }

        public IntraWarehouseOrderKey(IIntraWarehouseOrderKey intraWarehouseOrderKey)
            : this(intraWarehouseOrderKey.IntraWarehouseOrderKey_DateCreated, intraWarehouseOrderKey.IntraWarehouseOrderKey_Sequence) {}

        private IntraWarehouseOrderKey(DateTime dateCreated, int sequence)
        {
            _dateCreated = dateCreated;
            _sequence = sequence;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IIntraWarehouseOrderKey>

        protected override IIntraWarehouseOrderKey ParseImplementation(string keyValue)
        {
            if(keyValue == null) { throw new ArgumentNullException("keyValue"); }

            DateTime dateCreated;
            int sequence;

            var values = Regex.Split(keyValue, SEPARATOR);
            if(values.Count() != 2
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateCreated)
               || !int.TryParse(values[1], out sequence))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#.", keyValue, SEPARATOR));
            }

            return new IntraWarehouseOrderKey(dateCreated, sequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidIntraWarehouseOrderKey, inputValue);
        }

        public override IIntraWarehouseOrderKey Default
        {
            get { return Null; }
        }

        protected override IIntraWarehouseOrderKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IIntraWarehouseOrderKey key)
        {
            return string.Format("{1}{0}{2}", SEPARATOR, key.IntraWarehouseOrderKey_DateCreated.ToString("yyyyMMdd"), key.IntraWarehouseOrderKey_Sequence);
        }

        #endregion

        #region Implementation of IKey<IntraWarehouseOrder>.

        public Expression<Func<IntraWarehouseOrder, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.DateCreated == _dateCreated &&
                        i.Sequence == _sequence);
            }
        }

        #endregion

        #region Implementation of IIntraWarehouseOrderKey.

        public DateTime IntraWarehouseOrderKey_DateCreated
        {
            get { return _dateCreated; }
        }

        public int IntraWarehouseOrderKey_Sequence
        {
            get { return _sequence; }
        }

        #endregion

        #region Equality Overrides.

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            return ReferenceEquals(this, obj) || Equals(obj as IIntraWarehouseOrderKey);
        }

        protected bool Equals(IIntraWarehouseOrderKey other)
        {
            return other != null &&
                   _dateCreated.Equals(other.IntraWarehouseOrderKey_DateCreated) &&
                   _sequence == other.IntraWarehouseOrderKey_Sequence;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _dateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _sequence;
                return hashCode;
            }
        }

        #endregion

        public static IIntraWarehouseOrderKey Null = new NullIntraWarehouseOrderKey();

        private class NullIntraWarehouseOrderKey : IIntraWarehouseOrderKey
        {
            public DateTime IntraWarehouseOrderKey_DateCreated
            {
                get { return DateTime.MinValue; }
            }

            public int IntraWarehouseOrderKey_Sequence
            {
                get { return 0; }
            }
        }
    }

    public static class IIntraWarehouseOrderKeyExtensions
    {
        public static IntraWarehouseOrderKey ToIntraWarehouseOrderKey(this IIntraWarehouseOrderKey key)
        {
            return new IntraWarehouseOrderKey(key);
        }
    }
}