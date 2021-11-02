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
    public class LotAttributeDefectKey : EntityKeyBase.Of<ILotAttributeDefectKey>, IKey<LotAttributeDefect>, ILotAttributeDefectKey
    {
        #region fields and constructors

        private const string SEPARATOR = "-";
        private readonly DateTime _dateCreated;
        private readonly int _dateSequence;
        private readonly int _lotTypeId;
        private readonly int _defectId;
        private readonly string _attributeShortName;

        public LotAttributeDefectKey() { }

        public LotAttributeDefectKey(ILotDefectKey defectKey, IAttributeNameKey attributeNameKey)
            : this(defectKey.LotKey_DateCreated, defectKey.LotKey_DateSequence, defectKey.LotKey_LotTypeId, defectKey.LotDefectKey_Id, attributeNameKey.AttributeNameKey_ShortName) { }

        public LotAttributeDefectKey(ILotAttributeDefectKey attributeDefectKey)
            : this(attributeDefectKey.LotKey_DateCreated, attributeDefectKey.LotKey_DateSequence, attributeDefectKey.LotKey_LotTypeId, attributeDefectKey.LotDefectKey_Id, attributeDefectKey.AttributeNameKey_ShortName) { }

        private LotAttributeDefectKey(DateTime lotDateCreated, int lotDateSequence, int lotTypeId, int defectId, string attributeShortName)
        {
            _dateCreated = lotDateCreated;
            _dateSequence = lotDateSequence;
            _lotTypeId = lotTypeId;
            _defectId = defectId;
            _attributeShortName = attributeShortName;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<ILotProductSpecDefectKey>

        protected override ILotAttributeDefectKey ParseImplementation(string keyValue)
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
            if(values.Count() != 5
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateCreated)
               || !int.TryParse(values[1], out dateSequence)
               || !int.TryParse(values[2], out lotTypeId)
               || !int.TryParse(values[3], out defectId))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#{1}#{1}#.", keyValue, SEPARATOR));
            }

            return new LotAttributeDefectKey(dateCreated, dateSequence, lotTypeId, defectId, values[4]);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidLotProductSpecDefectKey, inputValue);
        }

        public override ILotAttributeDefectKey Default
        {
            get { return Null; }
        }

        protected override ILotAttributeDefectKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(ILotAttributeDefectKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}", SEPARATOR, key.LotKey_DateCreated.ToString("yyyyMMdd"), key.LotKey_DateSequence, key.LotKey_LotTypeId, key.LotDefectKey_Id, key.AttributeNameKey_ShortName);
        }

        #endregion

        #region Implementation of IKey<ILotProductSpecDefectKey>

        public Expression<Func<LotAttributeDefect, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.LotDateCreated == _dateCreated &&
                        i.LotDateSequence == _dateSequence &&
                        i.LotTypeId == _lotTypeId &&
                        i.DefectId == _defectId &&
                        i.AttributeShortName == _attributeShortName);
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

        public string AttributeNameKey_ShortName { get { return _attributeShortName; } }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as ILotAttributeDefectKey != null && Equals(obj as ILotAttributeDefectKey);
        }

        protected bool Equals(ILotAttributeDefectKey other)
        {
            return _dateCreated.Equals(other.LotKey_DateCreated) &&
                   _dateSequence == other.LotKey_DateSequence &&
                   _lotTypeId == other.LotKey_LotTypeId &&
                   _defectId == other.LotDefectKey_Id &&
                   _attributeShortName == other.AttributeNameKey_ShortName;
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

        public static ILotAttributeDefectKey Null = new NullLotAttributeDefectKey();

        private class NullLotAttributeDefectKey : ILotAttributeDefectKey
        {
            #region Implementation of ILotKey

            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }

            public int LotKey_DateSequence { get { return 0; } }

            public int LotKey_LotTypeId { get { return 0; } }

            #endregion

            public int LotDefectKey_Id { get { return 0; } }

            public string AttributeNameKey_ShortName { get { return ""; } }
        }
    }

    public static class LotAttributeDefectKeyExtensions
    {
        public static LotAttributeDefectKey ToLotAttributeDefectKey(this ILotAttributeDefectKey k)
        {
            return new LotAttributeDefectKey(k);
        }
    }
}