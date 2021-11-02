using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal class CustomerNoteConductor
    {
        internal CustomerNoteConductor(ICompanyUnitOfWork companyUnitOfWork)
        {
            _companyUnitOfWork = companyUnitOfWork;
        }

        internal IResult<CustomerNote> Create(ICustomerKey customerKey, DateTime timestamp, ISetCustomerNoteParameters parameters)
        {
            var noteId = new EFUnitOfWorkHelper(_companyUnitOfWork).GetNextSequence<CustomerNote>(c => c.CustomerId == customerKey.CustomerKey_Id, c => c.NoteId);
            var note = _companyUnitOfWork.CustomerNoteRepository.Add(new CustomerNote
                {
                    CustomerId = customerKey.CustomerKey_Id,
                    NoteId = noteId
                });

            return SetCustomerNote(note, timestamp, parameters);
        }

        internal IResult<CustomerNote> Update(CustomerNoteKey customerNoteKey, DateTime timestamp, ISetCustomerNoteParameters parameters)
        {
            var customerNote = _companyUnitOfWork.CustomerNoteRepository.FindByKey(customerNoteKey);
            if(customerNote == null)
            {
                return new InvalidResult<CustomerNote>(null, string.Format(UserMessages.CustomerNoteNotFound, customerNoteKey));
            }

            return SetCustomerNote(customerNote, timestamp, parameters);
        }

        private IResult<CustomerNote> SetCustomerNote(CustomerNote note, DateTime timestamp, ISetCustomerNoteParameters parameters)
        {
            var employeeResult = new GetEmployeeCommand(_companyUnitOfWork).GetEmployee(parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<CustomerNote>();
            }

            note.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            note.TimeStamp = timestamp;

            note.Type = parameters.Type;
            note.Text = parameters.Text;
            note.Bold = parameters.Bold;

            return new SuccessResult<CustomerNote>(note);
        }

        private readonly ICompanyUnitOfWork _companyUnitOfWork;
    }
}