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
    public class ChileProductKey : EntityKeyBase.Of<IChileProductKey>, IKey<ChileProduct>, IChileProductKey
    {
        #region fields and constructors

        public static ChileProductKey FromProductKey(IProductKey productKey)
        {
            return new ChileProductKey(productKey.ProductKey_ProductId);
        }

        private readonly int _chileProductKey;

        private ChileProductKey(int chileProductKey)
        {
            _chileProductKey = chileProductKey;
        }

        public ChileProductKey() : this(Null) { }

        public ChileProductKey(IChileProductKey chileProductKey) : this(chileProductKey.ChileProductKey_ProductId) { }

        #endregion

        #region Overrides of EntityKeyBase.Of<IChileProductKey>

        protected override IChileProductKey ParseImplementation(string keyValue)
        {
            return new ChileProductKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidChileProductKey, inputValue);
        }

        public override IChileProductKey Default
        {
            get { return Null; }
        }

        protected override IChileProductKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IChileProductKey key)
        {
            return key.ChileProductKey_ProductId.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<ChileProduct>

        public Expression<Func<ChileProduct, bool>> FindByPredicate 
        { 
            get { return (p => p.Id == _chileProductKey); } 
        }

        #endregion

        #region Implementation of IChileProductKey

        public int ChileProductKey_ProductId
        {
            get { return _chileProductKey; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (ReferenceEquals(obj, this)) return true;
            return obj as IChileProductKey != null && Equals(obj as IChileProductKey);
        }

        protected bool Equals(IChileProductKey other)
        {
            return _chileProductKey == other.ChileProductKey_ProductId;
        }

        public override int GetHashCode()
        {
            return _chileProductKey;
        }

        public static IChileProductKey Null = new NullChileProductKey();
        
        private class NullChileProductKey : IChileProductKey
        {
            #region Implementation of IChileProductKey

            public int ChileProductKey_ProductId { get { return 0; } }

            #endregion
        }
    }

    public static class IChileProductKeyExtensions
    {
        public static ChileProductKey ToChileProductKey(this IChileProductKey k)
        {
            return new ChileProductKey(k);
        }
    }
}