using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotContractAllowanceExtensions
    {
        internal static LotContractAllowance SetContract(this LotContractAllowance allowance, IContractKey contract)
        {
            allowance.Contract = null;
            allowance.ContractYear = contract.ContractKey_Year;
            allowance.ContractSequence = contract.ContractKey_Sequence;
            return allowance;
        }

        internal static void AssertAreEqual(this LotContractAllowance expected, ILotContractAllowanceReturn result)
        {
            Assert.AreEqual(expected.ToContractKey().KeyValue, result.ContractKey);
            Assert.AreEqual(expected.Contract.TermBegin, result.TermBegin);
            Assert.AreEqual(expected.Contract.TermEnd, result.TermEnd);
        }
    }
}