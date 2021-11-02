using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal class UpdateContactCommand : ContactCommandBase
    {
        internal UpdateContactCommand(ICompanyUnitOfWork companyUnitOfWork) : base(companyUnitOfWork) { }

        internal IResult<Contact> Execute(UpdateContactCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var contact = CompanyUnitOfWork.ContactRepository.FindByKey(parameters.ContactKey, c => c.Addresses);
            if(contact == null)
            {
                return new FailureResult<Contact>(null, string.Format(UserMessages.ContactNotFound, parameters.ContactKey.KeyValue));
            }

            return SetContact(contact, parameters);
        }
    }
}