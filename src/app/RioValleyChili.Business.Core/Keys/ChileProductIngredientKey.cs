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
    public class ChileProductIngredientKey : EntityKeyBase.Of<IChileProductIngredientKey>, IKey<ChileProductIngredient>, IChileProductIngredientKey
    {
        #region fields and constructors

        private const string SEPARATOR = "-";

        private readonly int _chileProductId;

        private readonly int _additiveTypeId;

        public ChileProductIngredientKey() { }

        public ChileProductIngredientKey(IChileProductIngredientKey chileProductIngredientKey)
            : this(chileProductIngredientKey.ChileProductIngredientKey_ChileProductId, chileProductIngredientKey.ChileProductIngredientKey_AdditiveTypeId) { }

        public ChileProductIngredientKey(IChileProductKey chileProductKey, IAdditiveTypeKey additiveTypeKey)
            : this(chileProductKey.ChileProductKey_ProductId, additiveTypeKey.AdditiveTypeKey_AdditiveTypeId) { }

        private ChileProductIngredientKey(int chileProductId, int additiveTypeId)
        {
            _chileProductId = chileProductId;
            _additiveTypeId = additiveTypeId;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IChileProductIngredientKey>

        protected override IChileProductIngredientKey ParseImplementation(string keyValue)
        {
            if (keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }

            int chileProductId;
            int additiveTypeId;

            var values = Regex.Split(keyValue, SEPARATOR);
            if (values.Count() != 2
                || !int.TryParse(values[0], out chileProductId)
                || !int.TryParse(values[1], out additiveTypeId))
            {
                throw new FormatException(string.Format("The keyValue '{0}' was in an invalid format. Expected #{1}#.", keyValue, SEPARATOR));
            }

            return new ChileProductIngredientKey(chileProductId, additiveTypeId);
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return UserMessages.InvalidChileProductIngredientKey;
        }

        public override IChileProductIngredientKey Default
        {
            get { return Null; }
        }

        protected override IChileProductIngredientKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IChileProductIngredientKey key)
        {
            return string.Format("{1}{0}{2}", SEPARATOR, key.ChileProductIngredientKey_ChileProductId, key.ChileProductIngredientKey_AdditiveTypeId);
        }

        #endregion

        #region Implementation of IKey<ChileProductIngredient>

        public Expression<Func<ChileProductIngredient, bool>> FindByPredicate
        {
            get
            {
                return (i => i.ChileProductId == _chileProductId && i.AdditiveTypeId == _additiveTypeId);
            }
        }

        #endregion

        #region Implementation of IChileProductIngredientKey

        public int ChileProductIngredientKey_ChileProductId
        {
            get { return _chileProductId; }
        }
        public int ChileProductIngredientKey_AdditiveTypeId
        {
            get { return _additiveTypeId; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (ReferenceEquals(obj, this)) return true;
            return obj as IChileProductIngredientKey != null && Equals(obj as IChileProductIngredientKey);
        }

        private bool Equals(IChileProductIngredientKey other)
        {
            return other.ChileProductIngredientKey_AdditiveTypeId == ChileProductIngredientKey_AdditiveTypeId
                   && other.ChileProductIngredientKey_ChileProductId == ChileProductIngredientKey_ChileProductId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_chileProductId * 397) ^ _additiveTypeId;
            }
        }

        public static IChileProductIngredientKey Null = new NullChileProductIngredientKey();

        private class NullChileProductIngredientKey : IChileProductIngredientKey
        {
            #region Implementation of IChileProductIngredientKey

            public int ChileProductIngredientKey_ChileProductId { get { return 0; } }
            public int ChileProductIngredientKey_AdditiveTypeId { get { return 0; } }

            #endregion
        }
    }
}