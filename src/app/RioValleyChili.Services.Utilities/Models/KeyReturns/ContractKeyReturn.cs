using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ContractKeyReturn : IContractKey
    {
        internal string ContractKey { get { return this.ToContractKey(); } }
        public int ContractKey_Year { get; internal set; }
        public int ContractKey_Sequence { get; internal set; }
    }
}