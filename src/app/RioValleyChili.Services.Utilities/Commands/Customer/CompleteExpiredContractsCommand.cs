using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class CompleteExpiredContractsCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal CompleteExpiredContractsCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult<List<Contract>> Execute()
        {
            var now = DateTime.UtcNow;
            var contracts = _salesUnitOfWork.ContractRepository.Filter(c => now >= c.TermEnd && c.ContractStatus == ContractStatus.Confirmed).ToList()
                .Select(c =>
                    {
                        c.ContractStatus = ContractStatus.Completed;
                        return c;
                    }).ToList();
            return new SuccessResult<List<Contract>>(contracts);
        }
    }
}