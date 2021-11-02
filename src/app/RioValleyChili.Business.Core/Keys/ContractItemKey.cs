using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ContractItemKey : EntityKey<IContractItemKey>.With<int, int, int>, IKey<ContractItem>, IContractItemKey
    {
        public ContractItemKey() { }

        public ContractItemKey(IContractItemKey contractItemKey) : base(contractItemKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidContractItemKey, inputValue);
        }

        protected override IContractItemKey ConstructKey(int field0, int field1, int field2)
        {
            return new ContractItemKey { ContractKey_Year = field0, ContractKey_Sequence = field1, ContractItemKey_Sequence = field2 };
        }

        protected override With<int, int, int> DeconstructKey(IContractItemKey key)
        {
            return new ContractItemKey { ContractKey_Year = key.ContractKey_Year, ContractKey_Sequence = key.ContractKey_Sequence, ContractItemKey_Sequence = key.ContractItemKey_Sequence };
        }

        public Expression<Func<ContractItem, bool>> FindByPredicate { get { return c => c.ContractYear == Field0 && c.ContractSequence == Field1 && c.ContractItemSequence == Field2; } }

        public int ContractKey_Year { get { return Field0; } private set { Field0 = value; } }

        public int ContractKey_Sequence { get { return Field1; } private set { Field1 = value; } }

        public int ContractItemKey_Sequence { get { return Field2; } private set { Field2 = value; } }

        public static IContractItemKey Null = new ContractItemKey();
    }

    public static class IContractItemKeyExtensions
    {
        public static ContractItemKey ToContractItemKey(this IContractItemKey k)
        {
            return new ContractItemKey(k);
        }
    }
}