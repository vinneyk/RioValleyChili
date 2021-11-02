using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class RemoveCustomerFromBrokerCommand
    {
        private readonly ICompanyUnitOfWork _companyUnitOfWork;

        internal RemoveCustomerFromBrokerCommand(ICompanyUnitOfWork companyUnitOfWork)
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
            if(broker.CompanyTypes.All(t =>t.CompanyTypeEnum != CompanyType.Broker))
            {
                return new InvalidResult(string.Format(UserMessages.CompanyNotOfType, brokerKey.KeyValue, CompanyType.Broker));
            }

            var customer = _companyUnitOfWork.CustomerRepository.FindByKey(customerKey, c => c.Broker);
            if(customer == null)
            {
                return new InvalidResult(string.Format(UserMessages.CustomerNotFound, customerKey.KeyValue));
            }

            if(!brokerKey.Equals(new CompanyKey(customer.Broker)))
            {
                return new InvalidResult(string.Format(UserMessages.CustomerNotOfBroker, customerKey.KeyValue, brokerKey.KeyValue));
            }

            var rvcBroker = _companyUnitOfWork.CompanyRepository.Filter(c => c.Name == Data.Models.StaticRecords.StaticCompanies.RVCBroker.Name).FirstOrDefault();
            if(rvcBroker == null)
            {
                throw new Exception("Default RVC Broker Company not found.");
            }

            customer.Broker = rvcBroker;
            customer.BrokerId = rvcBroker.Id;

            return new SuccessResult();
        }
    }
}