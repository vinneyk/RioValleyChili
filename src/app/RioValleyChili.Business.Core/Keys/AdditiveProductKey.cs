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
    public class AdditiveProductKey : EntityKeyBase.Of<IAdditiveProductKey>, IKey<AdditiveProduct>, IAdditiveProductKey
    {
        #region constructors and fields

        public static AdditiveProductKey FromProductKey(IProductKey productKey)
        {
            return new AdditiveProductKey(productKey.ProductKey_ProductId);
        }

        private readonly int _id;

        public AdditiveProductKey() : this(0) { }
        
        public AdditiveProductKey(IAdditiveProductKey additiveProductKey) : this(additiveProductKey.AdditiveProductKey_Id) { }
        
        private AdditiveProductKey(int id)
        {
            _id = id;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IAdditiveProductKey>

        protected override IAdditiveProductKey ParseImplementation(string keyValue)
        {
            var id = int.Parse(keyValue);
            return new AdditiveProductKey(id);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return UserMessages.InvalidAdditiveProductKey;
        }

        public override IAdditiveProductKey Default
        {
            get { return Null; }
        }

        protected override IAdditiveProductKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IAdditiveProductKey key)
        {
            return key.AdditiveProductKey_Id.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<AdditiveProduct>

        public Expression<Func<AdditiveProduct, bool>> FindByPredicate
        {
            get { return (p => p.Id == _id); }
        }

        #endregion

        #region Implementation of IAdditiveProductKey

        public int AdditiveProductKey_Id
        {
            get { return _id; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as IAdditiveProductKey != null && Equals(obj as IAdditiveProductKey);
        }

        protected bool Equals(IAdditiveProductKey other)
        {
            return _id == other.AdditiveProductKey_Id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static IAdditiveProductKey Null = new NullAdditiveProductKey();

        private class NullAdditiveProductKey : IAdditiveProductKey
        {
            #region Implementation of IAdditiveProductKey

            public int AdditiveProductKey_Id { get { return 0; } }

            #endregion
        }
    }

    public static class IAdditiveProductKeyExtensions
    {
        public static AdditiveProductKey ToAdditiveProductKey(this IAdditiveProductKey k)
        {
            return new AdditiveProductKey(k);
        }
    }
}