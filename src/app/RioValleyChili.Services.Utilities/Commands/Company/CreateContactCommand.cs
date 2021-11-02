using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal class CreateContactCommand : ContactCommandBase
    {
        internal CreateContactCommand(ICompanyUnitOfWork companyUnitOfWork) : base(companyUnitOfWork) { }

        internal IResult<Contact> Execute(CreateContactCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var company = CompanyUnitOfWork.CompanyRepository.FindByKey(parameters.CompanyKey);
            if(company == null)
            {
                return new InvalidResult<Contact>(null, string.Format(UserMessages.CompanyNotFound, parameters.CompanyKey.KeyValue));
            }

            var nextSequence = new EFUnitOfWorkHelper(CompanyUnitOfWork).GetNextSequence<Contact>(c => c.CompanyId == company.Id, c => c.ContactId);
            var contact = new Contact
                {
                    CompanyId = company.Id,
                    ContactId = nextSequence,
                    Addresses = new List<ContactAddress>()
                };

            return SetContact(CompanyUnitOfWork.ContactRepository.Add(contact), parameters);
        }
    }
}