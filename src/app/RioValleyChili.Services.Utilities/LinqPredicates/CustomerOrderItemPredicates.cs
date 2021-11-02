using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class CustomerOrderItemPredicates
    {
        internal static Expression<Func<SalesOrderItem, Contract, bool>> ByContract()
        {
            return (i, c) => i.ContractYear == c.ContractYear && i.ContractSequence == c.ContractSequence;
        }

        internal static Expression<Func<SalesOrderItem, ContractItem, bool>> ByContractItem()
        {
            return (i, c) => i.ContractYear == c.ContractYear && i.ContractSequence == c.ContractSequence && i.ContractItemSequence == c.ContractItemSequence;
        }
    }
}