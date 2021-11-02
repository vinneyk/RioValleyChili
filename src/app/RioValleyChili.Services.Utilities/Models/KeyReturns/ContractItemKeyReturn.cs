using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ContractItemKeyReturn : IContractItemKey
    {
        internal string ContractItemKey { get { return new ContractItemKey(this).KeyValue; } }
        internal string ContractKey { get { return new ContractKey(this).KeyValue; } }

        public int ContractKey_Year { get; internal set; }
        public int ContractKey_Sequence { get; internal set; }
        public int ContractItemKey_Sequence { get; internal set; }
    }

    internal class NullableContractItemKeyReturn : IContractItemKey
    {
        internal string ContractItemKey { get { return IsNull ? null : new ContractItemKey(this); } }
        internal string ContractKey { get { return IsNull ? null : new ContractKey(this); } }

        internal int? ContractKey_Year { get; set; }
        internal int? ContractKey_Sequence { get; set; }
        internal int? ContractItemKey_Sequence { get; set; }
        internal bool IsNull { get { return ContractKey_Year == null || ContractKey_Sequence == null || ContractItemKey_Sequence == null; } }

        int IContractKey.ContractKey_Year { get { return ContractKey_Year.Value; } }
        int IContractKey.ContractKey_Sequence { get { return ContractKey_Sequence.Value; } }
        int IContractItemKey.ContractItemKey_Sequence { get { return ContractItemKey_Sequence.Value; } }
    }
}