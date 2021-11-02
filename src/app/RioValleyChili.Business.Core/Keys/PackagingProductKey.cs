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
    public class PackagingProductKey : EntityKeyBase.Of<IPackagingProductKey>, IKey<PackagingProduct>, IPackagingProductKey
    {
        #region fields and constructors

        public static PackagingProductKey FromProductKey(IProductKey productKey)
        {
            return new PackagingProductKey(productKey.ProductKey_ProductId);
        }

        private readonly int _packagingProductId;

        public PackagingProductKey() { }

        public PackagingProductKey(IPackagingProductKey packagingProductKey) : this(packagingProductKey.PackagingProductKey_ProductId) { }

        private PackagingProductKey(int packagingProductId)
        {
            _packagingProductId = packagingProductId;
        }


        #endregion

        #region Overrides of NewEntityKeyBase.Of<IPackagingProductKey>

        protected override IPackagingProductKey ParseImplementation(string keyValue)
        {
            return new PackagingProductKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidPackagingProductKey, inputValue);
        }

        public override IPackagingProductKey Default
        {
            get { return Null; }
        }

        protected override IPackagingProductKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IPackagingProductKey key)
        {
            return key.PackagingProductKey_ProductId.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<PackagingProduct>

        public Expression<Func<PackagingProduct, bool>> FindByPredicate 
        {
            get { return (p => p.Id == PackagingProductKey_ProductId); }
        }

        #endregion

        #region Implementation of IPackagingProductKey

        public int PackagingProductKey_ProductId
        {
            get { return _packagingProductId; }
        }

        #endregion

        #region Equality Overrides
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IPackagingProductKey != null && Equals(obj as IPackagingProductKey);
        }

        protected bool Equals(IPackagingProductKey other)
        {
            return _packagingProductId == other.PackagingProductKey_ProductId;
        }

        public override int GetHashCode()
        {
            return _packagingProductId;
        }

        #endregion

        public static IPackagingProductKey Null = new NullPackagingProductKey();

        private class NullPackagingProductKey : IPackagingProductKey
        {
            #region Implementation of IPackagingProductKey

            public int PackagingProductKey_ProductId { get { return 0; } }

            #endregion
        }
    }

    public static class IPackagingProductKeyExtensions
    {
        public static PackagingProductKey ToPackagingProductKey(this IPackagingProductKey k)
        {
            return new PackagingProductKey(k);
        }
    }
}