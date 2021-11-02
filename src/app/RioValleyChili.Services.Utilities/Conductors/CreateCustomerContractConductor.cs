using System;
using System.Data.Entity;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Notebook;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CreateCustomerContractConductor
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        internal CreateCustomerContractConductor(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        internal IResult<Contract> Execute(CreateCustomerContractCommandParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            if(timeStamp == null) { throw new ArgumentNullException("timeStamp"); }

            var customer = _salesUnitOfWork.CustomerRepository.FindByKey(parameters.CustomerKey, c => c.Company.Contacts.Select(n => n.Addresses), c => c.ProductCodes);
            if(customer == null)
            {
                return new InvalidResult<Contract>(null, string.Format(UserMessages.CustomerNotFound, parameters.CustomerKey.KeyValue));
            }

            var facilityKey = parameters.DefaultPickFromFacilityKey ?? new FacilityKey(GlobalKeyHelpers.RinconFacilityKey);
            var facility = _salesUnitOfWork.FacilityRepository.FindByKey(facilityKey);
            if(facility == null)
            {
                return new InvalidResult<Contract>(null, string.Format(UserMessages.FacilityNotFound, facilityKey.KeyValue));
            }

            var employeeResult = new GetEmployeeCommand(_salesUnitOfWork).GetEmployee(parameters.CreateCustomerContractParameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<Contract>();
            }

            var commentsNotebookResult = new CreateNotebookCommand(_salesUnitOfWork).Execute(timeStamp, employeeResult.ResultingObject, parameters.CreateCustomerContractParameters.Comments);
            if(!commentsNotebookResult.Success)
            {
                return commentsNotebookResult.ConvertTo<Contract>();
            }

            var contractYear = timeStamp.Year;
            var contractSequence = new EFUnitOfWorkHelper(_salesUnitOfWork).GetNextSequence<Contract>(c => c.ContractYear == contractYear, c => c.ContractSequence);
            var contract = _salesUnitOfWork.ContractRepository.Add(new Contract
                {
                    EmployeeId = employeeResult.ResultingObject.EmployeeId,
                    TimeStamp = timeStamp,
                    TimeCreated = timeStamp,

                    ContractYear = contractYear,
                    ContractSequence = contractSequence,

                    CustomerId = customer.Id,
                    BrokerId = customer.BrokerId,
                    DefaultPickFromWarehouseId = facility.Id,

                    CommentsDate = commentsNotebookResult.ResultingObject.Date,
                    CommentsSequence = commentsNotebookResult.ResultingObject.Sequence,

                    Customer = customer,
                    ContractId = _salesUnitOfWork.ContractRepository.SourceQuery.Select(c => c.ContractId).DefaultIfEmpty(0).Max() + 1
                });

            return new UpdateCustomerContractConductor(_salesUnitOfWork).SetCustomerContract(contract, employeeResult.ResultingObject, timeStamp, parameters);
        }
    }
}
