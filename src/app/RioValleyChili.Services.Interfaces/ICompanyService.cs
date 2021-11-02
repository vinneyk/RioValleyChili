using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface ICompanyService
    {
        IResult<string> CreateCompany(ICreateCompanyParameters parameters);

        IResult UpdateCompany(IUpdateCompanyParameters parameters);

        IResult<ICompanyDetailReturn> GetCompany(string companyKey);

        IResult<IQueryable<ICompanySummaryReturn>> GetCompanies(FilterCompanyParameters parameters = null);

        #region contacts methods

        IResult<IQueryable<IContactSummaryReturn>> GetContacts(FilterContactsParameters parameters = null);

        IResult<IContactSummaryReturn> GetContact(string contactKey);

        IResult UpdateContact(IUpdateContactParameters parameters);

        IResult<string> CreateContact(ICreateContactParameters parameters);

        IResult DeleteContact(string contactKey);

        #endregion

        #region customer note methods

        IResult<ICustomerCompanyNoteReturn> GetCustomerNote(string customerNoteKey);

        IResult<IEnumerable<string>> GetDistinctCustomerNoteTypes();
            
        IResult<string> CreateCustomerNote(ICreateCustomerNoteParameters parameters);

        IResult UpdateCustomerNote(IUpdateCustomerNoteParameters parameters);

        IResult DeleteCustomerNote(string customerNoteKey);

        #endregion
    }
}