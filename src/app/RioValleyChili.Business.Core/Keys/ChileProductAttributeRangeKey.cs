using System;
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
    public class ChileProductAttributeRangeKey : EntityKeyBase.Of<IChileProductAttributeRangeKey>, IKey<ChileProductAttributeRange>, IChileProductAttributeRangeKey
    {
        #region Constructors and Fields

        private const string SEPARATOR = "-";
        private readonly int _chileProductId;
        private readonly string _attributeShortName;

        public ChileProductAttributeRangeKey() : this(Null) { }

        public ChileProductAttributeRangeKey(IChileProductAttributeRangeKey chileProductKey)
            : this(chileProductKey.ChileProductKey_ProductId, chileProductKey.AttributeNameKey_ShortName) { }
        
        public ChileProductAttributeRangeKey(IChileProductKey chileProductKey, IAttributeNameKey attributeNameKey)
            : this(chileProductKey.ChileProductKey_ProductId, attributeNameKey.AttributeNameKey_ShortName) { }

        private ChileProductAttributeRangeKey(int chileProductId, string attributeShortName)
        {
            _chileProductId = chileProductId;
            _attributeShortName = attributeShortName;
        }

        #endregion

        #region Overrides of EntityKeyBase.Of<IChileProductAttributeRangeKey>

        protected override IChileProductAttributeRangeKey ParseImplementation(string keyValue)
        {
            if(keyValue == null) { throw new ArgumentNullException("keyValue"); }

            int chileProductId;

            var values = Regex.Split(keyValue, SEPARATOR);
            if(values.Count() != 2
                || !int.TryParse(values[0], out chileProductId))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected #{1}string.", keyValue, SEPARATOR));
            }

            return new ChileProductAttributeRangeKey(chileProductId, values[1]);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidChileProductAttributeRangeKey, inputValue);
        }

        public override IChileProductAttributeRangeKey Default
        {
            get { return Null; }
        }

        protected override IChileProductAttributeRangeKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IChileProductAttributeRangeKey key)
        {
            return string.Format("{1}{0}{2}", SEPARATOR, key.ChileProductKey_ProductId, key.AttributeNameKey_ShortName);
        }

        #endregion

        #region Implementation of IKey<ChileProductAttributeRange>

        public Expression<Func<ChileProductAttributeRange, bool>> FindByPredicate
        {
            get { return (r => r.ChileProductId == _chileProductId && r.AttributeShortName == _attributeShortName); }
        }

        #endregion

        #region Implementation of IChileProductAttributeRangeKey

        public int ChileProductKey_ProductId
        {
            get { return _chileProductId; }
        }

        public string AttributeNameKey_ShortName
        {
            get { return _attributeShortName; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as IChileProductAttributeRangeKey != null && Equals(obj as IChileProductAttributeRangeKey);
        }

        protected bool Equals(IChileProductAttributeRangeKey other)
        {
            return _chileProductId == other.ChileProductKey_ProductId && _attributeShortName == other.AttributeNameKey_ShortName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_chileProductId * 397) ^ _attributeShortName.GetHashCode();
            }
        }

        public static IChileProductAttributeRangeKey Null = new NullChileProductAttributeRangeKey();

        private class NullChileProductAttributeRangeKey : IChileProductAttributeRangeKey
        {
            public int ChileProductKey_ProductId { get { return 0; } }

            public string AttributeNameKey_ShortName { get { return ""; } }
        }
    }

    public static class IChileProductAttributeRangeKeyExtensions
    {
        public static ChileProductAttributeRangeKey ToChileProductAttributeRangeKey(this IChileProductAttributeRangeKey k)
        {
            return new ChileProductAttributeRangeKey(k);
        }
    }
}