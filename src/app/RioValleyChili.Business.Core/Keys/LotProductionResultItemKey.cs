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
    public class LotProductionResultItemKey : EntityKeyBase.Of<ILotProductionResultItemKey>, IKey<LotProductionResultItem>, ILotProductionResultItemKey
    {
        #region Fields and Constructors

        private const char SEPARATOR = '-';

        private readonly DateTime _lotDateCreated;
        private readonly int _lotDateSequence;
        private readonly int _lotTypeId;
        private readonly int _resultItemSequence;

        public LotProductionResultItemKey() { }

        private LotProductionResultItemKey(DateTime lotDateCreated, int lotDateSequence, int lotTypeId, int resultItemSequence)
        {
            _lotDateCreated = lotDateCreated;
            _lotDateSequence = lotDateSequence;
            _lotTypeId = lotTypeId;
            _resultItemSequence = resultItemSequence;
        }

        public LotProductionResultItemKey(ILotProductionResultItemKey lotProductionResultItemKey) : this(lotProductionResultItemKey.LotKey_DateCreated, lotProductionResultItemKey.LotKey_DateSequence, lotProductionResultItemKey.LotKey_LotTypeId, lotProductionResultItemKey.ProductionResultItemKey_Sequence) { }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IProductionResultKey>.

        protected override ILotProductionResultItemKey ParseImplementation(string keyValue)
        {
            if (keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }

            DateTime lotDateCreated;
            int lotDateSequence;
            int lotTypeId;
            int lotItemSequence;

            var values = keyValue.Split(SEPARATOR);
            if (values.Length != 4 ||
                !DateTime.TryParseExact(values[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out lotDateCreated) ||
                !int.TryParse(values[1], out lotDateSequence) ||
                !int.TryParse(values[2], out lotTypeId) ||
                !int.TryParse(values[3], out lotItemSequence))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected yyyyMMdd{1}#{1}#{1}#.", keyValue, SEPARATOR));
            }

            return new LotProductionResultItemKey(lotDateCreated, lotDateSequence, lotTypeId, lotItemSequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidProductionResultItemKey, inputValue);
        }

        public override ILotProductionResultItemKey Default
        {
            get { return Null; }
        }

        protected override ILotProductionResultItemKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(ILotProductionResultItemKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}", SEPARATOR, key.LotKey_DateCreated.ToString("yyyyMMdd"), key.LotKey_DateSequence, key.LotKey_LotTypeId, key.ProductionResultItemKey_Sequence);
        }

        #endregion

        #region Implementation of IKey<IProductionResultItemKey>

        public Expression<Func<LotProductionResultItem, bool>> FindByPredicate
        {
            get
            {
                return i =>
                       i.LotDateCreated == _lotDateCreated &&
                       i.LotDateSequence == _lotDateSequence &&
                       i.LotTypeId == _lotTypeId &&
                       i.ResultItemSequence == _resultItemSequence;
            }
        }

        #endregion

        #region Implemenation of IProductionResultItemKey.

        public DateTime LotKey_DateCreated { get { return _lotDateCreated; } }
        public int LotKey_DateSequence { get { return _lotDateSequence; } }
        public int LotKey_LotTypeId { get { return _lotTypeId; } }
        public int ProductionResultItemKey_Sequence { get { return _resultItemSequence; } }

        #endregion

        #region Equality overrides.

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as ILotProductionResultItemKey != null && Equals(obj as ILotProductionResultItemKey);
        }

        public bool Equals(ILotProductionResultItemKey other)
        {
            return _lotDateCreated.Equals(other.LotKey_DateCreated) &&
                _lotDateSequence == other.LotKey_DateSequence &&
                _lotTypeId.Equals(other.LotKey_LotTypeId) &&
                _resultItemSequence == other.ProductionResultItemKey_Sequence;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _lotDateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _lotDateSequence;
                hashCode = (hashCode * 397) ^ _lotTypeId;
                hashCode = (hashCode * 397) ^ _resultItemSequence;
                return hashCode;
            }
        }

        #endregion

        public static ILotProductionResultItemKey Null = new NullLotProductionResultItemKey();

        private class NullLotProductionResultItemKey : ILotProductionResultItemKey
        {
            #region Implementation of IProductionResultKey.

            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }
            public int LotKey_DateSequence { get { return 0; } }
            public int LotKey_LotTypeId { get { return 0; } }
            public int ProductionResultItemKey_Sequence { get { return 0; } }

            #endregion
        }
    }
}