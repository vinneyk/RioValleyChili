using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class ContractKeyReservationHelper : KeyReservationHelperBase<ContractKey, IContractKey>
    {
        private readonly SequenceHelper<int> _sequenceHelper = new SequenceHelper<int>();

        protected override IContractKey GetNextKey(IContractKey keyInterface)
        {
            return new Key
                {
                    ContractKey_Year = keyInterface.ContractKey_Year,
                    ContractKey_Sequence = _sequenceHelper.GetNextSequence(keyInterface.ContractKey_Year, keyInterface.ContractKey_Sequence + 1)
                };
        }

        public class Key : IContractKey
        {
            public int ContractKey_Year { get; set; }
            public int ContractKey_Sequence { get; set; }
        }
    }
}