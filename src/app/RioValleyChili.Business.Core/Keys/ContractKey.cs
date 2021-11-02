using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ContractKey : EntityKey<IContractKey>.With<int, int>, IKey<Contract>, IContractKey
    {
        public ContractKey() { }

        public ContractKey(IContractKey contractKey) : base(contractKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidContractKey, inputValue);
        }

        protected override IContractKey ConstructKey(int field0, int field1)
        {
            return new ContractKey { ContractKey_Year = field0, ContractKey_Sequence = field1 };
        }

        protected override With<int, int> DeconstructKey(IContractKey key)
        {
            return new ContractKey { ContractKey_Year = key.ContractKey_Year, ContractKey_Sequence = key.ContractKey_Sequence };
        }

        public Expression<Func<Contract, bool>> FindByPredicate { get { return c => c.ContractYear == Field0 && c.ContractSequence == Field1; } }

        public int ContractKey_Year { get { return Field0; } private set { Field0 = value; } }

        public int ContractKey_Sequence { get { return Field1; } private set { Field1 = value; } }

        public static IContractKey Null = new ContractKey();
    }

    public static class IContractKeyExtensions
    {
        public static ContractKey ToContractKey(this IContractKey k)
        {
            return new ContractKey(k);
        }
    }
}