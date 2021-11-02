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
    public class LocationKey : EntityKeyBase.Of<ILocationKey>, IKey<Location>, ILocationKey
    {
        #region fields and constructors

        private readonly int _locationId;

        public LocationKey() : this(Null) { }

        public LocationKey(int locationId)
        {
            _locationId = locationId;
        }

        public LocationKey(ILocationKey locationKey)
            : this(locationKey.LocationKey_Id) { }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IWorkTypeKey>

        protected override ILocationKey ParseImplementation(string keyValue)
        {
            return new LocationKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidLocationKey, inputValue);
        }

        public override ILocationKey Default
        {
            get { return Null; }
        }

        protected override ILocationKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(ILocationKey key)
        {
            return key.LocationKey_Id.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<WorkType>

        public Expression<Func<Location, bool>> FindByPredicate
        {
            get { return (l => l.Id == _locationId); }
        }

        #endregion

        #region Implementation of ILocationKey

        public int LocationKey_Id
        {
            get { return _locationId; }
        }

        #endregion

        #region Equality Overrides
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as ILocationKey != null && Equals(obj as ILocationKey);
        }

        protected bool Equals(ILocationKey other)
        {
            return _locationId == other.LocationKey_Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _locationId;
            }
        }

        #endregion

        public static ILocationKey Null = new NullLocationKey();

        private class NullLocationKey : ILocationKey
        {
            #region Implementation of ILocationKey

            public int LocationKey_Id { get { return 0; } }

            #endregion
        }
    }

    public static class ILocationKeyExtensions
    {
        public static LocationKey ToLocationKey(this ILocationKey k)
        {
            return new LocationKey(k);
        }
    }
}