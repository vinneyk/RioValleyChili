using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotContractAllowanceProjectors
    {
        internal static Expression<Func<LotContractAllowance, LotContractAllowanceReturn>> Select()
        {
            var contractKey = ContractProjectors.SelectKey();
            var customerKey = CustomerProjectors.SelectKey();

            return a => new LotContractAllowanceReturn
                {
                    ContractKeyReturn = contractKey.Invoke(a.Contract),
                    TermBegin = a.Contract.TermBegin,
                    TermEnd = a.Contract.TermEnd,
                    CustomerKeyReturn = customerKey.Invoke(a.Contract.Customer),
                    CustomerName = a.Contract.Customer.Company.Name
                };
        }

        internal static Expression<Func<LotContractAllowance, ContractKeyReturn>> SelectContractKey()
        {
            return a => new ContractKeyReturn
                {
                    ContractKey_Year = a.ContractYear,
                    ContractKey_Sequence = a.ContractSequence
                };
        }
    }
}