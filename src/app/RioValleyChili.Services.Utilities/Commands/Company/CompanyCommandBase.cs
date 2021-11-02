using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal abstract class CompanyCommandBase
    {
        protected readonly ICompanyUnitOfWork CompanyUnitOfWork;

        internal CompanyCommandBase(ICompanyUnitOfWork companyUnitOfWork)
        {
            if(companyUnitOfWork == null) { throw new ArgumentNullException("companyUnitOfWork"); }
            CompanyUnitOfWork = companyUnitOfWork;
        }

        protected IResult<Data.Models.Company> SetCompany<T>(Data.Models.Company company, DateTime timestamp, SetCompanyCommandParameters<T> parameters)
            where T : ISetCompanyParameters
        {
            var employeeResult = new GetEmployeeCommand(CompanyUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<Data.Models.Company>();
            }

            company.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            company.TimeStamp = timestamp;
            company.Active = parameters.Parameters.Active;

            var setTypesResult = SetCompanyTypes(company, parameters.Parameters.CompanyTypes, parameters.BrokerKey);
            if(!setTypesResult.Success)
            {
                return setTypesResult.ConvertTo<Data.Models.Company>();
            }

            return new SuccessResult<Data.Models.Company>(company);
        }

        private IResult SetCompanyTypes(Data.Models.Company company, IEnumerable<CompanyType> companyTypes, CompanyKey brokerKey)
        {
            if(company == null) { throw new ArgumentNullException("company"); }

            company.CompanyTypes = company.CompanyTypes ?? new List<CompanyTypeRecord>();
            var setTypes = companyTypes == null ? new List<CompanyType>() : companyTypes.Distinct().ToList();
            var typesToRemove = company.CompanyTypes.Where(t => !setTypes.Contains(t.CompanyTypeEnum)).ToList();

            setTypes.Where(t => company.CompanyTypes.All(e => e.CompanyTypeEnum != t)).Select(t => new CompanyTypeRecord
                {
                    CompanyId = company.Id,
                    CompanyTypeEnum = t
                }).ToList().ForEach(n => company.CompanyTypes.Add(n));;
            typesToRemove.ForEach(t => company.CompanyTypes.Remove(t));

            var setCustomerRecordResult = SetCustomerRecord(company, brokerKey);
            if(!setCustomerRecordResult.Success)
            {
                return setCustomerRecordResult;
            }

            return new SuccessResult();
        }

        private IResult SetCustomerRecord(Data.Models.Company company, CompanyKey brokerKey)
        {
            if(company == null) { throw new ArgumentNullException("company"); }

            var customer = CompanyUnitOfWork.CustomerRepository.FindByKey(new CustomerKey(company));
            if(company.CompanyTypes.Any(t => t.CompanyTypeEnum == CompanyType.Customer))
            {
                if(customer == null)
                {
                    customer = CompanyUnitOfWork.CustomerRepository.Add(new Data.Models.Customer
                        {
                            Id = company.Id,
                            Company = company,
                        });
                }

                var setBrokerResult = SetCustomerBroker(customer, brokerKey);
                if(!setBrokerResult.Success)
                {
                    return setBrokerResult;
                }
            }

            return new SuccessResult();
        }

        private IResult<Data.Models.Customer> SetCustomerBroker(Data.Models.Customer customer, CompanyKey brokerKey)
        {
            if(customer == null) { throw new ArgumentNullException("customer"); }

            Data.Models.Company broker;
            if(brokerKey == null)
            {
                broker = CompanyUnitOfWork.CompanyRepository.Filter(c => c.Name == StaticCompanies.RVCBroker.Name).FirstOrDefault();
                if(broker == null)
                {
                    throw new Exception("Default RVC Broker Company not found.");
                }
            }
            else
            {
                var brokerResult = new GetCompanyCommand(CompanyUnitOfWork).Execute(brokerKey, CompanyType.Broker);
                if(!brokerResult.Success)
                {
                    return brokerResult.ConvertTo<Data.Models.Customer>();
                }

                broker = brokerResult.ResultingObject;
            }

            customer.BrokerId = broker.Id;

            return new SuccessResult<Data.Models.Customer>(customer);
        }
    }
}