using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using RioValleyChili.Business.Core.Helpers;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class LotKey : EntityKeyBase.Of<ILotKey>, ILotKey,
        IKey<Lot>, IKey<AdditiveLot>, IKey<ChileLot>, IKey<PackagingLot>, IKey<ProductionBatch>, IKey<LotAttributeDefect>, IKey<ChileLotProduction>, IKey<LotProductionResults>, IKey<ChileMaterialsReceived>
    {
        #region fields and constructors
        
        private readonly DateTime _dateCreated;
        private readonly int _dateSequence;
        private readonly int _lotTypeId;

        public LotKey() { }

        public LotKey(ILotKey lotKey) : this(lotKey.LotKey_DateCreated, lotKey.LotKey_DateSequence, lotKey.LotKey_LotTypeId) { }

        private LotKey(DateTime dateCreated, int dateSequence, int lotTypeId)
        {
            _dateCreated = dateCreated.Date;
            _dateSequence = dateSequence;
            _lotTypeId = lotTypeId;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<ILotKey>

        protected override ILotKey ParseImplementation(string keyValue)
        {
            if (keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }

            if(keyValue.Count() == 8)
            {
                keyValue = "0" + keyValue;
            }

            if(!keyValue.Contains(' '))
            {
                var match = Regex.Match(keyValue, @"(\d{2})(\d{2})(\d{3})(\d+)");
                if(match.Success)
                {
                    keyValue = string.Format("{0} {1} {2} {3}",
                                             match.Groups[1],
                                             match.Groups[2],
                                             match.Groups[3],
                                             match.Groups[4]);
                }
            }

            var splitStrings = keyValue.Split(' ');
            if (splitStrings.Count() != 4)
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected # yy ddd #.", keyValue));
            }

            var lotTypeId = int.Parse(splitStrings[0]);

            var dateCreated = new DateTime(StringHelper.GetYearFromString(splitStrings[1]), 1, 1)
                .AddDays(int.Parse(splitStrings[2]) - 1);

            var dateSequence = int.Parse(splitStrings[3]);

            return new LotKey(dateCreated, dateSequence, lotTypeId);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidLotKey, inputValue);
        }

        public override ILotKey Default
        {   
            get { return Null; }
        }

        protected override ILotKey Key
        {
            get { return new LotKey(this); }
        }

        protected override string BuildKeyValueImplementation(ILotKey key)
        {
            return string.Format("{0:00} {1:yy} {2:000} {3:00}", key.LotKey_LotTypeId, key.LotKey_DateCreated, key.LotKey_DateCreated.DayOfYear, key.LotKey_DateSequence);
        }

        #endregion

        public Expression<Func<TLot, bool>> GetPredicate<TLot>() where TLot : LotKeyEntityBase
        {
            return l => l.LotDateCreated == _dateCreated &&
                            l.LotDateSequence == _dateSequence &&
                            l.LotTypeId == _lotTypeId;
        }

        #region Implementation of IKey<TLot>

        public Expression<Func<Lot, bool>> FindByPredicate
        {
            get { return GetPredicate<Lot>(); }
        }

        Expression<Func<AdditiveLot, bool>> IKey<AdditiveLot>.FindByPredicate
        {
            get { return GetPredicate<AdditiveLot>(); }
        }

        Expression<Func<ChileLot, bool>> IKey<ChileLot>.FindByPredicate
        {
            get { return GetPredicate<ChileLot>(); }
        }

        Expression<Func<PackagingLot, bool>> IKey<PackagingLot>.FindByPredicate
        {
            get { return GetPredicate<PackagingLot>(); }
        }

        Expression<Func<LotAttributeDefect, bool>> IKey<LotAttributeDefect>.FindByPredicate
        {
            get { return GetPredicate<LotAttributeDefect>(); }
        }

        Expression<Func<ProductionBatch, bool>> IKey<ProductionBatch>.FindByPredicate
        {
            get { return GetPredicate<ProductionBatch>(); }
        }

        Expression<Func<ChileLotProduction, bool>> IKey<ChileLotProduction>.FindByPredicate
        {
            get { return GetPredicate<ChileLotProduction>(); }
        }

        Expression<Func<LotProductionResults, bool>> IKey<LotProductionResults>.FindByPredicate
        {
            get { return GetPredicate<LotProductionResults>(); }
        }

        Expression<Func<ChileMaterialsReceived, bool>> IKey<ChileMaterialsReceived>.FindByPredicate
        {
            get { return GetPredicate<ChileMaterialsReceived>(); }
        }

        #endregion

        #region Implementation of ILotKey

        public DateTime LotKey_DateCreated { get { return _dateCreated; } }

        public int LotKey_DateSequence { get { return _dateSequence; } }

        public int LotKey_LotTypeId { get { return _lotTypeId; } }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as ILotKey != null && Equals(obj as ILotKey);
        }

        public bool Equals(ILotKey lotKey)
        {
            return _lotTypeId.Equals(lotKey.LotKey_LotTypeId) &&
                   _dateCreated.Equals(lotKey.LotKey_DateCreated) &&
                   _dateSequence.Equals(lotKey.LotKey_DateSequence);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _lotTypeId;
                hashCode = (hashCode * 397) ^ _dateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ _dateSequence;
                hashCode = (hashCode * 397) ^ _lotTypeId;
                return hashCode;
            }
        }

        #endregion

        public static ILotKey Null = new NullLotKey();

        private class NullLotKey : ILotKey
        {
            #region Implementation of ILotKey

            public DateTime LotKey_DateCreated { get { return DateTime.MinValue; } }
            public int LotKey_DateSequence { get { return 0; } }
            public int LotKey_LotTypeId { get { return 0; } }

            #endregion
        }
    }

    public static class ILotKeyExtensions
    {
        public static LotKey ToLotKey(this ILotKey k)
        {
            return new LotKey(k);
        }
    }
}