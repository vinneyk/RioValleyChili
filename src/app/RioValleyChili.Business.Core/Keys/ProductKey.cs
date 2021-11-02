using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ProductKey : EntityKey<IProductKey>.With<int>, IProductKey, IKey<Product>, IKey<AdditiveProduct>, IKey<ChileProduct>, IKey<PackagingProduct>
    {
        public ProductKey() { }

        public ProductKey(IProductKey productKey) : base(productKey) { }

        public ProductKey(IChileProductKey chileProductKey) : base(chileProductKey.ChileProductKey_ProductId) { }

        public ProductKey(IPackagingProductKey packagingProductKey) : base(packagingProductKey.PackagingProductKey_ProductId) { }

        public ProductKey(IAdditiveProductKey additiveProductKey) : base(additiveProductKey.AdditiveProductKey_Id) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidProductKey, inputValue);
        }

        protected override IProductKey ConstructKey(int field0)
        {
            return new ProductKey { Field0 = field0 };
        }

        protected override With<int> DeconstructKey(IProductKey key)
        {
            return new ProductKey { Field0 = key.ProductKey_ProductId };
        }

        Expression<Func<Product, bool>> IKey<Product>.FindByPredicate
        {
            get { return p => p.Id == Field0; }
        }

        Expression<Func<AdditiveProduct, bool>> IKey<AdditiveProduct>.FindByPredicate
        {
            get { return p => p.Id == Field0; }
        }

        Expression<Func<ChileProduct, bool>> IKey<ChileProduct>.FindByPredicate
        {
            get { return p => p.Id == Field0; }
        }

        Expression<Func<PackagingProduct, bool>> IKey<PackagingProduct>.FindByPredicate
        {
            get { return p => p.Id == Field0; }
        }

        public int ProductKey_ProductId { get { return Field0; } }

        public static IProductKey Null { get { return new ProductKey(); } }
    }

    public static class IProductKeyExtensions
    {
        public static ProductKey ToProductKey(this IProductKey k)
        {
            return new ProductKey(k);
        }
    }
}