using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class UpdateCustomerContractConductor
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal UpdateCustomerContractConductor(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult<Contract> Execute(UpdateCustomerContractCommandParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var contract = _salesUnitOfWork.ContractRepository.FindByKey(parameters.ContractKey,
                c => c.Broker,
                c => c.ContractItems);
            if(contract == null)
            {
                return new InvalidResult<Contract>(null, string.Format(UserMessages.CustomerContractNotFound, parameters.ContractKey.KeyValue));
            }

            var customer = _salesUnitOfWork.CustomerRepository.FindByKey(parameters.CustomerKey,
                c => c.Broker,
                c => c.Company.Contacts.Select(n => n.Addresses),
                c => c.ProductCodes);
            if(customer == null)
            {
                return new InvalidResult<Contract>(null, string.Format(UserMessages.CustomerNotFound, parameters.CustomerKey.KeyValue));
            }

            contract.Customer = customer;
            contract.CustomerId = customer.Id;

            if(parameters.DefaultPickFromFacilityKey != null && !parameters.DefaultPickFromFacilityKey.Equals(contract))
            {
                var facility = _salesUnitOfWork.FacilityRepository.FindByKey(parameters.DefaultPickFromFacilityKey);
                if(facility == null)
                {
                    return new InvalidResult<Contract>(null, string.Format(UserMessages.FacilityNotFound, parameters.DefaultPickFromFacilityKey.KeyValue));
                }

                contract.DefaultPickFromFacility = facility;
                contract.DefaultPickFromWarehouseId = facility.Id;
            }

            if(!parameters.BrokerKey.Equals(contract.Broker))
            {
                if(!parameters.BrokerKey.Equals(customer.Broker))
                {
                    return new InvalidResult<Contract>(null, string.Format(UserMessages.ContractBrokerIsNotOfCustomer, parameters.BrokerKey.KeyValue, parameters.CustomerKey.KeyValue));
                }

                contract.Broker = null;
                contract.BrokerId = parameters.BrokerKey.CompanyKey_Id;
            }

            var employeeResult = new GetEmployeeCommand(_salesUnitOfWork).GetEmployee(parameters.UpdateCustomerContractParameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<Contract>();
            }

            return SetCustomerContract(contract, employeeResult.ResultingObject, timeStamp, parameters);
        }

        internal IResult<Contract> SetCustomerContract(Contract contract, IEmployeeKey employeeKey, DateTime timeStamp, IUpdateCustomerContractCommandParameters parameters)
        {
            if(contract == null) { throw new ArgumentNullException("contract"); }
            if(timeStamp == null) { throw new ArgumentNullException("timeStamp"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(contract.ContractStatus == ContractStatus.Completed && parameters.ContractStatus == ContractStatus.Completed)
            {
                return new InvalidResult<Contract>(null, string.Format(UserMessages.CustomerContractCompletedCannotChange, new ContractKey(contract).KeyValue));
            }

            contract.EmployeeId = employeeKey.EmployeeKey_Id;
            contract.TimeStamp = timeStamp;

            contract.ContactName = parameters.ContactName;
            contract.FOB = parameters.FOB;
            contract.ContactAddress = parameters.ContactAddress ?? new Address();

            contract.ContractType = parameters.ContractType;
            contract.ContractStatus = parameters.ContractStatus;
            contract.PaymentTerms = parameters.PaymentTerms;
            contract.CustomerPurchaseOrder = parameters.CustomerPurchaseOrder;
            contract.ContractDate = parameters.ContractDate.Date;
            contract.TermBegin = parameters.TermBegin;
            contract.TermEnd = parameters.TermEnd;

            contract.NotesToPrint = parameters.NotesToPrint;
            
            var setItemsResult = new SetContractItemsCommand(_salesUnitOfWork).Execute(contract, parameters.ContractItems);
            if(!setItemsResult.Success)
            {
                return setItemsResult.ConvertTo((Contract) null);
            }

            return new SuccessResult<Contract>(contract);
        }
    }
}