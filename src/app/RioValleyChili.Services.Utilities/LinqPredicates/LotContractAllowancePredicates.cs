using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class LotContractAllowancePredicates
    {
        internal static Expression<Func<LotContractAllowance, bool>> ByContractKey(IContractKey key)
        {
            return a => a.ContractYear == key.ContractKey_Year && a.ContractSequence == key.ContractKey_Sequence;
        }
    }
}

