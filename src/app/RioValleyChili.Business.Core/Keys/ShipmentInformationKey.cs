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
    public class ShipmentInformationKey : EntityKeyBase.Of<IShipmentInformationKey>, IKey<ShipmentInformation>, IShipmentInformationKey
    {
        #region Fields and Constructors.

        private const string SEPARATOR = "-";

        private readonly DateTime _dateCreated;
        private readonly int _sequence;

        public ShipmentInformationKey() : this(Null) { }

        public ShipmentInformationKey(IShipmentInformationKey shipmentInformationKey)
            : this(shipmentInformationKey.ShipmentInfoKey_DateCreated, shipmentInformationKey.ShipmentInfoKey_Sequence) { }

        private ShipmentInformationKey(DateTime dateCreated, int sequence)
        {
            _dateCreated = dateCreated;
            _sequence = sequence;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IPickedInventoryKey>

        protected override IShipmentInformationKey ParseImplementation(string keyValue)
        {
            if (keyValue == null) { throw new ArgumentNullException("keyValue"); }

            DateTime dateCreated;
            int sequence;

            var values = Regex.Split(keyValue, SEPARATOR);
            if (values.Count() != 2
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateCreated)
               || !int.TryParse(values[1], out sequence))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#.", keyValue, SEPARATOR));
            }

            return new ShipmentInformationKey(dateCreated, sequence);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidShipmentInfoKey, inputValue);
        }

        public override IShipmentInformationKey Default
        {
            get { return Null; }
        }

        protected override IShipmentInformationKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IShipmentInformationKey key)
        {
            return string.Format("{1}{0}{2}", SEPARATOR, key.ShipmentInfoKey_DateCreated.ToString("yyyyMMdd"), key.ShipmentInfoKey_Sequence);
        }

        #endregion

        #region Implementation of IKey<ShipmentInformation>.

        public Expression<Func<ShipmentInformation, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.DateCreated == _dateCreated &&
                        i.Sequence == _sequence);
            }
        }

        #endregion

        #region Implementation of IShipmentInformationKey.

        public DateTime ShipmentInfoKey_DateCreated
        {
            get { return _dateCreated; }
        }

        public int ShipmentInfoKey_Sequence
        {
            get { return _sequence; }
        }

        #endregion

        #region Equality Overrides.

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return ReferenceEquals(this, obj) || Equals(obj as IShipmentInformationKey);
        }

        protected bool Equals(IShipmentInformationKey other)
        {
            return other != null &&
                   _dateCreated.Equals(other.ShipmentInfoKey_DateCreated) &&
                   _sequence == other.ShipmentInfoKey_Sequence;
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

        public static IShipmentInformationKey Null = new NullShipmentInformationKey();

        private class NullShipmentInformationKey : IShipmentInformationKey
        {
            public DateTime ShipmentInfoKey_DateCreated
            {
                get { return DateTime.MinValue; }
            }

            public int ShipmentInfoKey_Sequence
            {
                get { return 0; }
            }
        }
    }

    public static class IShipmentInformationKeyExtensions
    {
        public static ShipmentInformationKey ToShipmentInformationKey(this IShipmentInformationKey k)
        {
            return new ShipmentInformationKey(k);
        }
    }
}