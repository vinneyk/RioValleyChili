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
    public class InventoryPickOrderKey : EntityKeyBase.Of<IInventoryPickOrderKey>, IKey<InventoryPickOrder>, IInventoryPickOrderKey
    {
        #region Fields and Constructors.

        private const string SEPARATOR = "-";

        private readonly DateTime _dateCreated;
        private readonly int _sequence;

        public InventoryPickOrderKey() : this(Null) { }

        public InventoryPickOrderKey(IInventoryPickOrderKey inventoryPickOrderKey)
            : this(inventoryPickOrderKey.InventoryPickOrderKey_DateCreated, inventoryPickOrderKey.InventoryPickOrderKey_Sequence) {}

        private InventoryPickOrderKey(DateTime dateCreated, int sequence)
        {
            _dateCreated = dateCreated;
            _sequence = sequence;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IPickedInventoryKey>

        protected override IInventoryPickOrderKey ParseImplementation(string keyValue)
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

            return new InventoryPickOrderKey(dateCreated, sequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidInventoryPickOrderKey, inputValue);
        }

        public override IInventoryPickOrderKey Default
        {
            get { return Null; }
        }

        protected override IInventoryPickOrderKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IInventoryPickOrderKey key)
        {
            return string.Format("{1}{0}{2}", SEPARATOR, key.InventoryPickOrderKey_DateCreated.ToString("yyyyMMdd"), key.InventoryPickOrderKey_Sequence);
        }

        #endregion

        #region Implementation of IKey<InventoryPickOrder>.

        public Expression<Func<InventoryPickOrder, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.DateCreated == _dateCreated &&
                        i.Sequence == _sequence);
            }
        }

        #endregion

        #region Implementation of IInventoryPickOrderKey.

        public DateTime InventoryPickOrderKey_DateCreated
        {
            get { return _dateCreated; }
        }

        public int InventoryPickOrderKey_Sequence
        {
            get { return _sequence; }
        }

        #endregion

        #region Equality Overrides.

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            return ReferenceEquals(this, obj) || Equals(obj as IInventoryPickOrderKey);
        }

        protected bool Equals(IInventoryPickOrderKey other)
        {
            return other != null &&
                   _dateCreated.Equals(other.InventoryPickOrderKey_DateCreated) &&
                   _sequence == other.InventoryPickOrderKey_Sequence;
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

        public static IInventoryPickOrderKey Null = new NullInventoryPickOrderKey();

        private class NullInventoryPickOrderKey : IInventoryPickOrderKey
        {
            public DateTime InventoryPickOrderKey_DateCreated
            {
                get { return DateTime.MinValue; }
            }

            public int InventoryPickOrderKey_Sequence
            {
                get { return 0; }
            }
        }
    }
}