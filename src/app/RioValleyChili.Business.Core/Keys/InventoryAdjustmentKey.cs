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
    public class InventoryAdjustmentKey : EntityKeyBase.Of<IInventoryAdjustmentKey>, IKey<InventoryAdjustment>, IInventoryAdjustmentKey
    {
        #region Constructors and Fields.

        private const string SEPARATOR = "-";
        private readonly DateTime _adjustmentDate;
        private readonly int _sequence;

        public InventoryAdjustmentKey() : this(Null) { }

        public InventoryAdjustmentKey(IInventoryAdjustmentKey key) : this(key.InventoryAdjustmentKey_AdjustmentDate, key.InventoryAdjustmentKey_Sequence) { }

        private InventoryAdjustmentKey(DateTime adjustmentDate, int sequence)
        {
            _adjustmentDate = adjustmentDate;
            _sequence = sequence;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IInventoryAdjustmentKey>.

        protected override IInventoryAdjustmentKey ParseImplementation(string keyValue)
        {
            if(keyValue == null ) { throw new ArgumentNullException("keyValue"); }

            DateTime adjustmentDate;
            int sequence;

            var values = Regex.Split(keyValue, SEPARATOR);
            if(values.Count() != 2
                || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out adjustmentDate)
                || !int.TryParse(values[1], out sequence))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#.", keyValue, SEPARATOR));
            }

            return new InventoryAdjustmentKey(adjustmentDate, sequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidInventoryAdjustmentKey, inputValue);
        }

        public override IInventoryAdjustmentKey Default
        {
            get { return Null; }
        }

        protected override IInventoryAdjustmentKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IInventoryAdjustmentKey key)
        {
            return string.Format("{1}{0}{2}", SEPARATOR, key.InventoryAdjustmentKey_AdjustmentDate.ToString("yyyyMMdd"), key.InventoryAdjustmentKey_Sequence);
        }

        #endregion

        #region Implemenation of IKey<InventoryAdjustment>.

        public Expression<Func<InventoryAdjustment, bool>> FindByPredicate
        {
            get
            {
                return a =>
                       a.AdjustmentDate == _adjustmentDate &&
                       a.Sequence == _sequence;
            }
        }

        #endregion

        #region Implementation of IInventoryAdjustmentKey.

        public DateTime InventoryAdjustmentKey_AdjustmentDate { get { return _adjustmentDate; } }

        public int InventoryAdjustmentKey_Sequence { get { return _sequence; } }

        #endregion

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as IInventoryAdjustmentKey != null && Equals(obj as IInventoryAdjustmentKey);
        }

        protected bool Equals(IInventoryAdjustmentKey other)
        {
            return _adjustmentDate == other.InventoryAdjustmentKey_AdjustmentDate &&
                   _sequence == other.InventoryAdjustmentKey_Sequence;
        }

        public override int GetHashCode()
        {
            int hashCode = _adjustmentDate.GetHashCode();
            hashCode = (hashCode * 397) ^ _sequence;
            return hashCode;
        }

        public static IInventoryAdjustmentKey Null = new NullInventoryTransactionKey();

        private class NullInventoryTransactionKey : IInventoryAdjustmentKey 
        {
            public DateTime InventoryAdjustmentKey_AdjustmentDate { get { return DateTime.MinValue; } }

            public int InventoryAdjustmentKey_Sequence { get { return 0; } }
        }
    }
}