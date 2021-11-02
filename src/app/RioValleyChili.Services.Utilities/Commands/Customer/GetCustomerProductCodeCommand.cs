using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class GetCustomerProductCodeCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal GetCustomerProductCodeCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult<CustomerProductCodeReturn> Execute(CustomerKey customerKey, ChileProductKey chileProductKey)
        {
            var predicate = new CustomerProductCodeKey(customerKey, chileProductKey).FindByPredicate;
            var select = CustomerProductCodeProjectors.Select();
            var code = _salesUnitOfWork.CustomerProductCodeRepository.Filter(predicate).Select(select).FirstOrDefault();

            return new SuccessResult<CustomerProductCodeReturn>(code);
        }
    }
}