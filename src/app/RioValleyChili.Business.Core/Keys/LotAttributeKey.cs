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
    public class LotAttributeKey : EntityKeyBase.Of<ILotAttributeKey>, IKey<LotAttribute>, ILotAttributeKey
    {
        #region Fields and Constructors.

        private const string SEPARATOR = "-";

        private readonly DateTime _lotDateCreated;
        private readonly int _lotDateSequence;
        private readonly int _lotTypeId;
        private readonly string _attributeShortName;

        public LotAttributeKey() : this(Null) { }

        public LotAttributeKey(ILotAttributeKey lotAttributeKey) : this(lotAttributeKey, lotAttributeKey) { }

        public LotAttributeKey(ILotKey lotKey, IAttributeNameKey attributeNameKey) : this(lotKey.LotKey_DateCreated, lotKey.LotKey_DateSequence, lotKey.LotKey_LotTypeId, attributeNameKey.AttributeNameKey_ShortName) { }

        private LotAttributeKey(DateTime lotDateCreated, int lotDateSequence, int lotTypeId, string attributeShortName)
        {
            _lotDateCreated = lotDateCreated;
            _lotDateSequence = lotDateSequence;
            _lotTypeId = lotTypeId;
            _attributeShortName = attributeShortName;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IPickedInventoryItemKey>

        protected override ILotAttributeKey ParseImplementation(string keyValue)
        {
            if(keyValue == null) { throw new ArgumentNullException("keyValue"); }

            DateTime lotDateCreated;
            int lotDateSequence;
            int lotTypeId;

            var values = Regex.Split(keyValue, SEPARATOR);
            if(values.Count() != 4
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out lotDateCreated)
               || !int.TryParse(values[1], out lotDateSequence)
               || !int.TryParse(values[2], out lotTypeId))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#{1}#{1}string.", keyValue, SEPARATOR));
            }

            return new LotAttributeKey(lotDateCreated, lotDateSequence, lotTypeId, values[3]);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidLotAttributeKey, inputValue);
        }

        public override ILotAttributeKey Default
        {
            get { return Null; }
        }

        protected override ILotAttributeKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(ILotAttributeKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}", SEPARATOR, key.LotKey_DateCreated.ToString("yyyyMMdd"), key.LotKey_DateSequence, key.LotKey_LotTypeId, key.AttributeNameKey_ShortName);
        }

        #endregion

        #region Implementation of IKey<LotAttribute>.

        public Expression<Func<LotAttribute, bool>> FindByPredicate
        {
            get
            {
                return (a =>
                        a.LotDateCreated == _lotDateCreated &&
                        a.LotDateSequence == _lotDateSequence &&
                        a.LotTypeId == _lotTypeId &&
                        a.AttributeShortName == _attributeShortName);
            }
        }

        #endregion

        #region Implementation of IPickedInventoryItemKey.

        public DateTime LotKey_DateCreated { get { return _lotDateCreated; } }

        public int LotKey_DateSequence { get { return _lotDateSequence; } }

        public int LotKey_LotTypeId { get { return _lotTypeId; } }

        public string AttributeNameKey_ShortName { get { return _attributeShortName; } }

        #endregion

        #region Equality Overrides.

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            return ReferenceEquals(this, obj) || Equals(obj as ILotAttributeKey);
        }

        protected bool Equals(ILotAttributeKey other)
        {
            return _lotDateCreated.Equals(other.LotKey_DateCreated) &&
                   _lotDateSequence == other.LotKey_DateSequence &&
                   _lotTypeId == other.LotKey_LotTypeId &&
                   string.Equals(_attributeShortName, other.AttributeNameKey_ShortName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _lotDateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _lotDateSequence;
                hashCode = (hashCode * 397) ^ _lotTypeId;
                hashCode = (hashCode * 397) ^ (_attributeShortName != null ? _attributeShortName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        public static ILotAttributeKey Null = new NullLotAttributeKey();

        private class NullLotAttributeKey : ILotAttributeKey
        {
            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }

            public int LotKey_DateSequence { get { return 0; } }

            public int LotKey_LotTypeId { get { return 0; } }

            public string AttributeNameKey_ShortName { get { return ""; } }
        }
    }

    public static class ILotAttributeKeyExtensions
    {
        public static LotAttributeKey ToLotAttributeKey(this ILotAttributeKey k)
        {
            return new LotAttributeKey(k);
        }
    }
}