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
    public class LotDefectKey : EntityKeyBase.Of<ILotDefectKey>, IKey<LotDefect>, ILotDefectKey
    {
        #region fields and constructors

        private const string SEPARATOR = "-";
        private readonly DateTime _dateCreated;
        private readonly int _dateSequence;
        private readonly int _lotTypeId;
        private readonly int _defectId;

        public LotDefectKey() { }

        public LotDefectKey(ILotDefectKey resolutionKey)
            : this(resolutionKey.LotKey_DateCreated, resolutionKey.LotKey_DateSequence, resolutionKey.LotKey_LotTypeId, resolutionKey.LotDefectKey_Id) { }

        private LotDefectKey(DateTime lotDateCreated, int lotDateSequence, int lotTypeId, int defectId)
        {
            _dateCreated = lotDateCreated;
            _dateSequence = lotDateSequence;
            _lotTypeId = lotTypeId;
            _defectId = defectId;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<ILotDefectKey>

        protected override ILotDefectKey ParseImplementation(string keyValue)
        {
            if(keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }

            DateTime dateCreated;
            int dateSequence;
            int lotTypeId;
            int defectId;

            var values = Regex.Split(keyValue, SEPARATOR);
            if(values.Count() != 4
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateCreated)
               || !int.TryParse(values[1], out dateSequence)
               || !int.TryParse(values[2], out lotTypeId)
               || !int.TryParse(values[3], out defectId))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#{1}#{1}#.", keyValue, SEPARATOR));
            }

            return new LotDefectKey(dateCreated, dateSequence, lotTypeId, defectId);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidLotDefectKey, inputValue);
        }

        public override ILotDefectKey Default
        {
            get { return Null; }
        }

        protected override ILotDefectKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(ILotDefectKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}", SEPARATOR, key.LotKey_DateCreated.ToString("yyyyMMdd"), key.LotKey_DateSequence, key.LotKey_LotTypeId, key.LotDefectKey_Id);
        }

        #endregion

        #region Implementation of IKey<LotDefect>

        public Expression<Func<LotDefect, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.LotDateCreated == _dateCreated &&
                        i.LotDateSequence == _dateSequence &&
                        i.LotTypeId == _lotTypeId &&
                        i.DefectId == _defectId);
            }
        }

        #endregion

        #region Key Interface Implementations

        #region Implementation of ILotKey

        public DateTime LotKey_DateCreated { get { return _dateCreated; } }

        public int LotKey_DateSequence { get { return _dateSequence; } }

        public int LotKey_LotTypeId { get { return _lotTypeId; } }

        #endregion

        public int LotDefectKey_Id { get { return _defectId; } }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as ILotDefectKey != null && Equals(obj as ILotDefectKey);
        }

        protected bool Equals(ILotDefectKey other)
        {
            return _dateCreated.Equals(other.LotKey_DateCreated) &&
                   _dateSequence == other.LotKey_DateSequence &&
                   _lotTypeId == other.LotKey_LotTypeId &&
                   _defectId == other.LotDefectKey_Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _dateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _dateSequence;
                hashCode = (hashCode * 397) ^ _lotTypeId;
                hashCode = (hashCode * 397) ^ _defectId;
                return hashCode;
            }
        }

        #endregion

        public static ILotDefectKey Null = new NullLotDefectKey();

        private class NullLotDefectKey : ILotDefectKey
        {
            #region Implementation of ILotKey

            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }

            public int LotKey_DateSequence { get { return 0; } }

            public int LotKey_LotTypeId { get { return 0; } }

            #endregion

            public int LotDefectKey_Id { get { return 0; } }
        }
    }

    public static class ILotDefectKeyExtensions
    {
        public static LotDefectKey ToLotDefectKey(this ILotDefectKey k)
        {
            return new LotDefectKey(k);
        }
    }
}