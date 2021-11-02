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
    public class FacilityKey : EntityKeyBase.Of<IFacilityKey>, IKey<Facility>, IFacilityKey
    {
        #region constructors and fields

        private readonly int _id;

        public FacilityKey() { }

        public FacilityKey(IFacilityKey facilityKey) : this(facilityKey.FacilityKey_Id) { }

        private FacilityKey(int id)
        {
            _id = id;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IFacilityKey>

        protected override IFacilityKey ParseImplementation(string keyValue)
        {
            return new FacilityKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidFacilityKey, inputValue);
        }

        protected override string BuildKeyValueImplementation(IFacilityKey key)
        {
            return key.FacilityKey_Id.ToString(CultureInfo.InvariantCulture);
        }

        public override IFacilityKey Default
        {
            get { return Null; }
        }

        protected override IFacilityKey Key
        {
            get { return this; }
        }

        #endregion

        #region Implementation of IKey<Facility>

        public Expression<Func<Facility, bool>> FindByPredicate
        {
            get { return w => w.Id == FacilityKey_Id; }
        }

        #endregion

        #region Implementation of IFacilityKey

        public int FacilityKey_Id { get { return _id; } }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IFacilityKey != null && Equals(obj as IFacilityKey);
        }

        protected bool Equals(IFacilityKey other)
        {
            return _id == other.FacilityKey_Id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        #endregion

        public static IFacilityKey Null = new NullFacilityKey();

        private class NullFacilityKey : IFacilityKey
        {
            #region Implementation of IFacilityKey

            public int FacilityKey_Id { get { return 0; } }

            #endregion
        }
    }

    public static class IFacilityKeyExtensions
    {
        public static FacilityKey ToFacilityKey(this IFacilityKey k)
        {
            return new FacilityKey(k);
        }
    }
}