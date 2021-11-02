using System;
using System.Globalization;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class ChileMaterialsReceivedItemKey : EntityKeyBase.Of<IChileMaterialsReceivedItemKey>, IKey<ChileMaterialsReceivedItem>, IChileMaterialsReceivedItemKey
    {
        #region fields and constructors

        private const char SEPARATOR = '-';

        private readonly DateTime _lotDateCreated;
        private readonly int _lotDateSequence;
        private readonly int _lotTypeId;
        private readonly int _itemSequence;

        public ChileMaterialsReceivedItemKey() : this(Null) { }

        public ChileMaterialsReceivedItemKey(IChileMaterialsReceivedItemKey lotKey)
            : this(lotKey.LotKey_DateCreated, lotKey.LotKey_DateSequence, lotKey.LotKey_LotTypeId, lotKey.ChileMaterialsReceivedKey_ItemSequence) { }

        private ChileMaterialsReceivedItemKey(DateTime lotDateCreated, int lotDateSequence, int lotTypeId, int itemSequence)
        {
            _lotDateCreated = lotDateCreated;
            _lotDateSequence = lotDateSequence;
            _lotTypeId = lotTypeId;
            _itemSequence = itemSequence;
        }

        #endregion

        #region Overrides of EntityKeyBase.Of<IDehydratedMaterialsReceivedItemKey>

        protected override IChileMaterialsReceivedItemKey ParseImplementation(string keyValue)
        {
            if(keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }

            DateTime lotDateCreated;
            int lotDateSequence;
            int lotTypeId;
            int itemSequence;

            var values = keyValue.Split(SEPARATOR);
            if(values.Length != 4 ||
               !DateTime.TryParseExact(values[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out lotDateCreated) ||
               !int.TryParse(values[1], out lotDateSequence) ||
               !int.TryParse(values[2], out lotTypeId) ||
               !int.TryParse(values[3], out itemSequence))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected yyyyMMdd{1}#{1}#{1}#.", keyValue, SEPARATOR));
            }

            return new ChileMaterialsReceivedItemKey(lotDateCreated, lotDateSequence, lotTypeId, itemSequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidDehydratedMaterialsReceivedItemKey, inputValue);
        }

        public override IChileMaterialsReceivedItemKey Default
        {
            get { return Null; }
        }

        protected override IChileMaterialsReceivedItemKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IChileMaterialsReceivedItemKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}", SEPARATOR, key.LotKey_DateCreated.ToString("yyyyMMdd"), key.LotKey_DateSequence, key.LotKey_LotTypeId, key.ChileMaterialsReceivedKey_ItemSequence);
        }

        #endregion

        #region Implementation of IKey<DehydrateMaterialsReceived>

        public Expression<Func<ChileMaterialsReceivedItem, bool>> FindByPredicate
        {
            get
            {
                return d =>
                       d.LotDateCreated == _lotDateCreated &&
                       d.LotDateSequence == _lotDateSequence &&
                       d.LotTypeId == _lotTypeId &&
                       d.ItemSequence == _itemSequence;
            }
        }

        #endregion

        #region Implementation of IDehydratedMaterialsReceivedItemKey

        public DateTime LotKey_DateCreated { get { return _lotDateCreated; } }

        public int LotKey_DateSequence { get { return _lotDateSequence; } }

        public int LotKey_LotTypeId { get { return _lotTypeId; } }

        public int ChileMaterialsReceivedKey_ItemSequence { get { return _itemSequence; } }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as IChileMaterialsReceivedItemKey != null && Equals(obj as IChileMaterialsReceivedItemKey);
        }

        protected bool Equals(IChileMaterialsReceivedItemKey other)
        {
            return _lotDateCreated.Equals(other.LotKey_DateCreated) && _lotDateSequence == other.LotKey_DateSequence && _lotTypeId == other.LotKey_LotTypeId && _itemSequence == other.ChileMaterialsReceivedKey_ItemSequence;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _lotDateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _lotDateSequence;
                hashCode = (hashCode * 397) ^ _lotTypeId;
                return hashCode;
            }
        }

        #endregion

        public static IChileMaterialsReceivedItemKey Null = new NullChileMaterialsReceivedKey();

        private class NullChileMaterialsReceivedKey : IChileMaterialsReceivedItemKey
        {
            #region Implementation of IDehydratedMaterialsReceivedItemKey

            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }
            public int LotKey_DateSequence { get { return 0; } }
            public int LotKey_LotTypeId { get { return 0; } }
            public int ChileMaterialsReceivedKey_ItemSequence { get { return 0; } }

            #endregion
        }
    }

    public static class ChileMaterialsReceivedItemKeyExtensions
    {
        public static ChileMaterialsReceivedItemKey ToChileMaterialsReceivedItemKey(this IChileMaterialsReceivedItemKey k)
        {
            return new ChileMaterialsReceivedItemKey(k);
        }
    }
}