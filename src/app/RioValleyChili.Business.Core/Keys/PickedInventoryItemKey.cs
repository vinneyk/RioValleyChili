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
    public class PickedInventoryItemKey : EntityKeyBase.Of<IPickedInventoryItemKey>, IKey<PickedInventoryItem>, IPickedInventoryItemKey
    {
        #region Fields and Constructors.

        private const string SEPARATOR = "-";

        private readonly DateTime _dateCreated;
        private readonly int _dateSequence;
        private readonly int _itemSequence;

        public PickedInventoryItemKey() : this(Null) { }

        public PickedInventoryItemKey(IPickedInventoryItemKey pickedInventoryItemKey)
            : this(pickedInventoryItemKey.PickedInventoryKey_DateCreated, pickedInventoryItemKey.PickedInventoryKey_Sequence, pickedInventoryItemKey.PickedInventoryItemKey_Sequence) {}

        private PickedInventoryItemKey(DateTime dateCreated, int dateSequence, int itemSequence)
        {
            _dateCreated = dateCreated;
            _dateSequence = dateSequence;
            _itemSequence = itemSequence;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IPickedInventoryItemKey>

        protected override IPickedInventoryItemKey ParseImplementation(string keyValue)
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

            return new PickedInventoryItemKey(dateCreated, orderSequence, itemSequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidPickedInventoryItemKey, inputValue);
        }

        public override IPickedInventoryItemKey Default
        {
            get { return Null; }
        }

        protected override IPickedInventoryItemKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IPickedInventoryItemKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}", SEPARATOR, key.PickedInventoryKey_DateCreated.ToString("yyyyMMdd"), key.PickedInventoryKey_Sequence, key.PickedInventoryItemKey_Sequence);
        }

        #endregion

        #region Implementation of IKey<PickedInventoryItem>.

        public Expression<Func<PickedInventoryItem, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.DateCreated == _dateCreated &&
                        i.Sequence == _dateSequence &&
                        i.ItemSequence == _itemSequence);
            }
        }

        #endregion

        #region Implementation of IPickedInventoryItemKey.

        public DateTime PickedInventoryKey_DateCreated
        {
            get { return _dateCreated; }
        }

        public int PickedInventoryKey_Sequence
        {
            get { return _dateSequence; }
        }

        public int PickedInventoryItemKey_Sequence
        {
            get { return _itemSequence; }
        }

        #endregion

        #region Equality Overrides.

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return ReferenceEquals(this, obj) || Equals(obj as IPickedInventoryItemKey);
        }

        protected bool Equals(IPickedInventoryItemKey other)
        {
            return other != null &&
                   _dateCreated.Equals(other.PickedInventoryKey_DateCreated) &&
                   _dateSequence == other.PickedInventoryKey_Sequence &&
                   _itemSequence == other.PickedInventoryItemKey_Sequence;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _dateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _dateSequence;
                hashCode = (hashCode * 397) ^ _itemSequence;
                return hashCode;
            }
        }

        #endregion

        public static IPickedInventoryItemKey Null = new NullPickedInventoryItemKey();

        private class NullPickedInventoryItemKey : IPickedInventoryItemKey
        {
            public DateTime PickedInventoryKey_DateCreated { get { return DateTime.MinValue; } }

            public int PickedInventoryKey_Sequence { get { return 0; } }

            public int PickedInventoryItemKey_Sequence { get { return 0; } }
        }
    }

    public static class IPickedInventoryItemKeyExtensions
    {
        public static PickedInventoryItemKey ToPickedInventoryItemKey(this IPickedInventoryItemKey k)
        {
            return new PickedInventoryItemKey(k);
        }
    }
}