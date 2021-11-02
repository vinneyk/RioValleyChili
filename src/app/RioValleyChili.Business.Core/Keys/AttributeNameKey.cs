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
    public class AttributeNameKey : EntityKeyBase.Of<IAttributeNameKey>, IKey<AttributeName>, IAttributeNameKey
    {
        #region Constructors and Fields

        private readonly string _shortName;

        public AttributeNameKey() : this("") { }
        
        public AttributeNameKey(IAttributeNameKey attribueName) : this(attribueName.AttributeNameKey_ShortName) { }

        private AttributeNameKey(string shortName)
        {
            _shortName = shortName;
        }

        #endregion

        #region Overrides of EntityKeyBase.Of<IAdditiveProductKey>

        protected override IAttributeNameKey ParseImplementation(string keyValue)
        {
            if(keyValue == null) { throw new ArgumentNullException("keyValue"); }
            
            return new AttributeNameKey(keyValue);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return UserMessages.InvalidAttributeNameKey;
        }

        public override IAttributeNameKey Default
        {
            get { return Null; }
        }

        protected override IAttributeNameKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IAttributeNameKey key)
        {
            return key.AttributeNameKey_ShortName.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<AttributeName>

        public Expression<Func<AttributeName, bool>> FindByPredicate
        {
            get { return (n => n.ShortName == _shortName); }
        }

        #endregion

        #region Implementation of IAttributeNameKey

        public string AttributeNameKey_ShortName
        {
            get { return _shortName; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(ReferenceEquals(obj, this)) return true;
            return obj as IAttributeNameKey != null && Equals(obj as IAttributeNameKey);
        }

        protected bool Equals(IAttributeNameKey other)
        {
            return _shortName == other.AttributeNameKey_ShortName;
        }

        public override int GetHashCode()
        {
            return _shortName.GetHashCode();
        }

        public static IAttributeNameKey Null = new NullAttributeNameKey();

        private class NullAttributeNameKey : IAttributeNameKey
        {
            #region Implementation of IAdditiveProductKey

            public string AttributeNameKey_ShortName { get { return ""; } }

            #endregion
        }
    }

    public static class IAttributeNameKeyExtensions
    {
        public static AttributeNameKey ToAttributeNameKey(this IAttributeNameKey k)
        {
            return new AttributeNameKey(k);
        }
    }
}