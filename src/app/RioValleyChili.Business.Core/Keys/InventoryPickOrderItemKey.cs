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
    public class InventoryPickOrderItemKey : EntityKeyBase.Of<IInventoryPickOrderItemKey>, IKey<InventoryPickOrderItem>, IInventoryPickOrderItemKey
    {
        #region Fields and Constructors.

        private const string SEPARATOR = "-";

        private readonly DateTime _dateCreated;
        private readonly int _orderSequence;
        private readonly int _itemSequence;

        public InventoryPickOrderItemKey() : this(Null) { }

        public InventoryPickOrderItemKey(IInventoryPickOrderItemKey inventoryPickOrderItemKey)
            : this(inventoryPickOrderItemKey.InventoryPickOrderKey_DateCreated, inventoryPickOrderItemKey.InventoryPickOrderKey_Sequence, inventoryPickOrderItemKey.InventoryPickOrderItemKey_Sequence) {}

        private InventoryPickOrderItemKey(DateTime dateCreated, int orderSequence, int itemSequence)
        {
            _dateCreated = dateCreated;
            _orderSequence = orderSequence;
            _itemSequence = itemSequence;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IPickedInventoryItemKey>

        protected override IInventoryPickOrderItemKey ParseImplementation(string keyValue)
        {
            if (keyValue == null) { throw new ArgumentNullException("keyValue"); }

            DateTime dateCreated;
            int orderSequence;
            int itemSequence;

            var values = Regex.Split(keyValue, SEPARATOR);
            if(values.Count() != 3
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateCreated)
               || !int.TryParse(values[1], out orderSequence)
               || !int.TryParse(values[2], out itemSequence))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#{1}#.", keyValue, SEPARATOR));
            }

            return new InventoryPickOrderItemKey(dateCreated, orderSequence, itemSequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidMovementOrderItemKey, inputValue);
        }

        public override IInventoryPickOrderItemKey Default
        {
            get { return Null; }
        }

        protected override IInventoryPickOrderItemKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IInventoryPickOrderItemKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}", SEPARATOR, key.InventoryPickOrderKey_DateCreated.ToString("yyyyMMdd"), key.InventoryPickOrderKey_Sequence, key.InventoryPickOrderItemKey_Sequence);
        }

        #endregion

        #region Implementation of IKey<InventoryPickOrderItem>.

        public Expression<Func<InventoryPickOrderItem, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.DateCreated == _dateCreated &&
                        i.OrderSequence == _orderSequence &&
                        i.ItemSequence == _itemSequence);
            }
        }

        #endregion

        #region Implementation of IInventoryPickOrderItemKey.

        public DateTime InventoryPickOrderKey_DateCreated
        {
            get { return _dateCreated; }
        }

        public int InventoryPickOrderKey_Sequence
        {
            get { return _orderSequence; }
        }

        public int InventoryPickOrderItemKey_Sequence
        {
            get { return _itemSequence; }
        }

        #endregion

        #region Equality Overrides.

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return ReferenceEquals(this, obj) || Equals(obj as IInventoryPickOrderItemKey);
        }

        protected bool Equals(IInventoryPickOrderItemKey other)
        {
            return other != null &&
                   _dateCreated.Equals(other.InventoryPickOrderKey_DateCreated) &&
                   _orderSequence == other.InventoryPickOrderKey_Sequence &&
                   _itemSequence == other.InventoryPickOrderItemKey_Sequence;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _dateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _orderSequence;
                hashCode = (hashCode * 397) ^ _itemSequence;
                return hashCode;
            }
        }

        #endregion

        public static IInventoryPickOrderItemKey Null = new NullInventoryPickOrderItemKey();

        private class NullInventoryPickOrderItemKey : IInventoryPickOrderItemKey
        {
            public DateTime InventoryPickOrderKey_DateCreated
            {
                get { return DateTime.MinValue; }
            }

            public int InventoryPickOrderKey_Sequence
            {
                get { return 0; }
            }

            public int InventoryPickOrderItemKey_Sequence
            {
                get { return 0; }
            }
        }
    }

    public static class IInventoryPickOrderItemKeyExtensions
    {
        public static InventoryPickOrderItemKey ToInventoryPickOrderItemKey(this IInventoryPickOrderItemKey k)
        {
            return new InventoryPickOrderItemKey(k);
        }
    }
}