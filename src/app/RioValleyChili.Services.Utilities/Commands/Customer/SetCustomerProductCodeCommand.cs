using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class SetCustomerProductCodeCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal SetCustomerProductCodeCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult Execute(CustomerKey customerKey, ChileProductKey chileProductKey, string code)
        {
            var customerProductCodeKey = new CustomerProductCodeKey(customerKey, chileProductKey);
            var productCode = _salesUnitOfWork.CustomerProductCodeRepository.FindByKey(customerProductCodeKey);
            if(productCode == null)
            {
                var customer = _salesUnitOfWork.CustomerRepository.FindByKey(customerKey);
                if(customer == null)
                {
                    return new InvalidResult(string.Format(UserMessages.CustomerNotFound, customerKey));
                }

                var chileProduct = _salesUnitOfWork.ChileProductRepository.FindByKey(chileProductKey);
                if(chileProduct == null)
                {
                    return new InvalidResult(string.Format(UserMessages.ChileProductNotFound, chileProductKey));
                }

                productCode = _salesUnitOfWork.CustomerProductCodeRepository.Add(new CustomerProductCode
                    {
                        CustomerId = customer.Id,
                        ChileProductId = chileProduct.Id
                    });
            }

            productCode.Code = code;
            
            return new SuccessResult();
        }
    }
}