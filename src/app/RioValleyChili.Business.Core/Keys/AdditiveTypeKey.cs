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
    public class AdditiveTypeKey : EntityKeyBase.Of<IAdditiveTypeKey>, IKey<AdditiveType>, IAdditiveTypeKey
    {
        #region fields and constructors

        private readonly int _additiveTypeId;

        public AdditiveTypeKey() { }

        public AdditiveTypeKey(IAdditiveTypeKey additiveTypeKey)
            : this(additiveTypeKey.AdditiveTypeKey_AdditiveTypeId) { }

        private AdditiveTypeKey(int additiveTypeId)
        {
            _additiveTypeId = additiveTypeId;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IAdditiveTypeKey>

        protected override IAdditiveTypeKey ParseImplementation(string keyValue)
        {
            return new AdditiveTypeKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return UserMessages.InvalidAdditiveTypeKey;
        }

        public override IAdditiveTypeKey Default
        {
            get { return Null; }
        }

        protected override IAdditiveTypeKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IAdditiveTypeKey key)
        {
            return key.AdditiveTypeKey_AdditiveTypeId.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<AdditiveType>

        public Expression<Func<AdditiveType, bool>> FindByPredicate 
        {
            get { return (i => i.Id == _additiveTypeId); }
        }

        #endregion

        #region Implementation of IAdditiveTypeKey

        public int AdditiveTypeKey_AdditiveTypeId
        {
            get { return _additiveTypeId; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(obj, this)){ return true; }
            return obj as IAdditiveTypeKey != null && Equals(obj as IAdditiveTypeKey);
        }

        protected bool Equals(IAdditiveTypeKey other)
        {
            return AdditiveTypeKey_AdditiveTypeId == other.AdditiveTypeKey_AdditiveTypeId;
        }

        public override int GetHashCode()
        {
            return _additiveTypeId;
        }

        public static IAdditiveTypeKey Null = new NullAdditiveTypeKey();

        private class NullAdditiveTypeKey : IAdditiveTypeKey
        {
            #region Implementation of IAdditiveTypeKey

            public int AdditiveTypeKey_AdditiveTypeId { get { return 0; } }

            #endregion
        }
    }

    public static class IAdditiveTypeKeyExtensions
    {
        public static AdditiveTypeKey ToAdditiveTypeKey(this IAdditiveTypeKey k)
        {
            return new AdditiveTypeKey(k);
        }
    }
}