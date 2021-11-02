using System;
using System.Globalization;
using System.IO;
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
    public class InventoryKey : EntityKeyBase.Of<IInventoryKey>, IKey<Inventory>, IInventoryKey
    {
        #region fields and constructors

        public const string SEPARATOR = ";";
        private readonly DateTime _dateCreated;
        private readonly int _dateSequence;
        private readonly int _lotTypeId;
        private readonly int _packagingProductId;
        private readonly int _locationId;
        private readonly int _inventoryTreatmentId;
        private readonly string _toteKey = " ";

        public InventoryKey() { }

        public InventoryKey(IInventoryKey inventoryKey)
            : this(inventoryKey.LotKey_DateCreated, inventoryKey.LotKey_DateSequence, inventoryKey.LotKey_LotTypeId, inventoryKey.PackagingProductKey_ProductId, inventoryKey.LocationKey_Id, inventoryKey.InventoryTreatmentKey_Id, inventoryKey.InventoryKey_ToteKey) { }

        public InventoryKey(ILotKey lotKey, IPackagingProductKey packagingProductKey, ILocationKey locationKey, IInventoryTreatmentKey treatmentKey, string toteKey)
            : this(lotKey.LotKey_DateCreated, lotKey.LotKey_DateSequence, lotKey.LotKey_LotTypeId, packagingProductKey.PackagingProductKey_ProductId, locationKey.LocationKey_Id, treatmentKey.InventoryTreatmentKey_Id, toteKey) { }

        private InventoryKey(DateTime lotDateCreated, int lotDateSequence, int lotTypeId, int packagingProductId, int locationId, int inventoryTreatmentId, string toteKey)
        {
            toteKey = toteKey ?? " ";
            if(toteKey.Contains(SEPARATOR)) { throw new InvalidDataException(string.Format(UserMessages.ToteKeyInvalidCharacter, SEPARATOR)); }

            _dateCreated = lotDateCreated;
            _dateSequence = lotDateSequence;
            _lotTypeId = lotTypeId;
            _packagingProductId = packagingProductId;
            _locationId = locationId;
            _inventoryTreatmentId = inventoryTreatmentId;
            _toteKey = toteKey;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IInventoryKey>

        protected override IInventoryKey ParseImplementation(string keyValue)
        {
            if(keyValue == null) { throw new ArgumentNullException("keyValue"); }

            DateTime dateCreated;
            int dateSequence;
            int lotTypeId;
            int packagingProductId;
            int locationId;
            int inventoryTreatmentId;
            string toteKey;

            var values = Regex.Split(keyValue, SEPARATOR);
            if (values.Count() != 7
               || !DateTime.TryParseExact(values[0], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateCreated)
               || !int.TryParse(values[1], out dateSequence)
               || !int.TryParse(values[2], out lotTypeId)
               || !int.TryParse(values[3], out packagingProductId)
               || !int.TryParse(values[4], out locationId)
               || !int.TryParse(values[5], out inventoryTreatmentId))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected date{1}#{1}#{1}#{1}#{1}#{1}string.", keyValue, SEPARATOR));
            }

            toteKey = values[6];

            return new InventoryKey(dateCreated, dateSequence, lotTypeId, packagingProductId, locationId, inventoryTreatmentId, toteKey);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidInventoryQuantityByLocationKey, inputValue);
        }

        public override IInventoryKey Default
        {
            get { return Null; }
        }

        protected override IInventoryKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IInventoryKey key)
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", SEPARATOR, key.LotKey_DateCreated.ToString("yyyyMMdd"), key.LotKey_DateSequence, key.LotKey_LotTypeId, key.PackagingProductKey_ProductId, key.LocationKey_Id, key.InventoryTreatmentKey_Id, key.InventoryKey_ToteKey);
        }

        #endregion

        #region Implementation of IKey<Inventory>

        public Expression<Func<Inventory, bool>> FindByPredicate
        {
            get
            {
                return (i =>
                        i.LotDateCreated == _dateCreated &&
                        i.LotDateSequence == _dateSequence &&
                        i.LotTypeId == _lotTypeId &&
                        i.PackagingProductId == _packagingProductId &&
                        i.LocationId == _locationId &&
                        i.TreatmentId == _inventoryTreatmentId &&
                        i.ToteKey == _toteKey);
            }
        }

        #endregion

        #region Key Interface Implementations

        #region Implementation of ILotKey

        public DateTime LotKey_DateCreated { get { return _dateCreated; } }

        public int LotKey_DateSequence { get { return _dateSequence; } }

        public int LotKey_LotTypeId { get { return _lotTypeId; } }

        #endregion

        public int LocationKey_Id { get { return _locationId; } }

        public int PackagingProductKey_ProductId { get { return _packagingProductId; } }

        public int InventoryTreatmentKey_Id { get { return _inventoryTreatmentId; } }

        public string InventoryKey_ToteKey { get { return _toteKey; } }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IInventoryKey != null && Equals(obj as IInventoryKey);
        }

        protected bool Equals(IInventoryKey other)
        {
            return _dateCreated.Equals(other.LotKey_DateCreated) && 
                _dateSequence == other.LotKey_DateSequence && 
                _lotTypeId == other.LotKey_LotTypeId && 
                _locationId == other.LocationKey_Id &&
                _packagingProductId == other.PackagingProductKey_ProductId &&
                _inventoryTreatmentId == other.InventoryTreatmentKey_Id &&
                _toteKey == other.InventoryKey_ToteKey;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _dateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _dateSequence;
                hashCode = (hashCode * 397) ^ _lotTypeId;
                hashCode = (hashCode * 397) ^ _locationId;
                hashCode = (hashCode * 397) ^ _packagingProductId;
                hashCode = (hashCode * 397) ^ _inventoryTreatmentId;
                hashCode = (hashCode * 397) ^ _toteKey.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        public static readonly IInventoryKey Null = new NullInventoryKey();

        private class NullInventoryKey : IInventoryKey
        {
            #region Implementation of ILotKey

            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }

            public int LotKey_DateSequence { get { return 0; } }

            public int LotKey_LotTypeId { get { return 0; } }

            #endregion

            public int LocationKey_Id { get { return 0; } }

            public int PackagingProductKey_ProductId { get { return 0; } }

            public int InventoryTreatmentKey_Id { get { return 0; } }

            public string InventoryKey_ToteKey { get { return " "; } }
        }
    }

    public static class IInventoryKeyExtensions
    {
        public static InventoryKey ToInventoryKey(this IInventoryKey k)
        {
            return new InventoryKey(k);
        }
    }
}