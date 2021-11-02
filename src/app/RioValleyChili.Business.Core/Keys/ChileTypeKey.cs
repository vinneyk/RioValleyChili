using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ChileTypeKey : EntityKey<IChileTypeKey>.With<int>, IChileTypeKey, IKey<ChileType>
    {
        public ChileTypeKey() { }

        public ChileTypeKey(IChileTypeKey key) : base(key) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidChileTypeKey, inputValue);
        }

        protected override IChileTypeKey ConstructKey(int field0)
        {
            return new ChileTypeKey
                {
                    Field0 = field0
                };
        }

        protected override With<int> DeconstructKey(IChileTypeKey key)
        {
            return new ChileTypeKey
                {
                    ChileTypeKey_ChileTypeId = key.ChileTypeKey_ChileTypeId
                };
        }

        public Expression<Func<ChileType, bool>> FindByPredicate
        {
            get { return c => c.Id == Field0; }
        }

        public int ChileTypeKey_ChileTypeId { get { return Field0; } private set { Field0 = value; } }

        public static IChileTypeKey Null = new ChileTypeKey();
    }

    public static class IChileTypeKeyExtensions
    {
        public static ChileTypeKey ToChileTypeKey(this IChileTypeKey k)
        {
            return new ChileTypeKey(k);
        }
    }
}