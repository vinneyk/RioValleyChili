using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class AssignCustomerToBrokerCommand
    {
        private readonly ICompanyUnitOfWork _companyUnitOfWork;

        internal AssignCustomerToBrokerCommand(ICompanyUnitOfWork companyUnitOfWork)
        {
            if(companyUnitOfWork == null) { throw new ArgumentNullException("companyUnitOfWork"); }
            _companyUnitOfWork = companyUnitOfWork;
        }

        internal IResult Execute(CompanyKey brokerKey, CustomerKey customerKey)
        {
            if(brokerKey == null) { throw new ArgumentNullException("brokerKey"); }
            if(customerKey == null) { throw new ArgumentNullException("customerKey"); }

            var broker = _companyUnitOfWork.CompanyRepository.FindByKey(brokerKey, c => c.CompanyTypes);
            if(broker == null)
            {
                return new InvalidResult(string.Format(UserMessages.CompanyNotFound, brokerKey.KeyValue));
            }
            if(broker.CompanyTypes.All(t => t.CompanyTypeEnum != CompanyType.Broker))
            {
                return new InvalidResult(string.Format(UserMessages.CompanyNotOfType, brokerKey.KeyValue, CompanyType.Broker));
            }

            var customer = _companyUnitOfWork.CustomerRepository.FindByKey(customerKey);
            if(customer == null)
            {
                return new InvalidResult(string.Format(UserMessages.CustomerNotFound, customerKey.KeyValue));
            }

            customer.Broker = broker;
            customer.BrokerId = broker.Id;

            return new SuccessResult();
        }
    }
}