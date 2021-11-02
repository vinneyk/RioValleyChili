using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly CompanyServiceProvider _companyServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public CompanyService(CompanyServiceProvider companyServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(companyServiceProvider == null) { throw new ArgumentNullException("companyServiceProvider"); }
            _companyServiceProvider = companyServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        public IResult<string> CreateCompany(ICreateCompanyParameters parameters)
        {
            try
            {
                return _companyServiceProvider.CreateCompany(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateCompany(IUpdateCompanyParameters parameters)
        {
            try
            {
                return _companyServiceProvider.UpdateCompany(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<ICompanyDetailReturn> GetCompany(string companyKey)
        {
            try
            {
                return _companyServiceProvider.GetCompany(companyKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICompanyDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<ICompanySummaryReturn>> GetCompanies(FilterCompanyParameters parameters = null)
        {
            try
            {
                return _companyServiceProvider.GetCompanies(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ICompanySummaryReturn>>(null, ex.Message);
            }
        }

        #region company contacts

        public IResult<IQueryable<IContactSummaryReturn>> GetContacts(FilterContactsParameters parameters = null)
        {
            try
            {
                return _companyServiceProvider.GetContacts(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IContactSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<IContactSummaryReturn> GetContact(string contactKey)
        {
            try
            {
                return _companyServiceProvider.GetContact(contactKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IContactSummaryReturn>(null, ex.Message);
            }
        }

        public IResult UpdateContact(IUpdateContactParameters parameters)
        {
            try
            {
                return _companyServiceProvider.UpdateContact(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<string> CreateContact(ICreateContactParameters parameters)
        {
            try
            {
                return _companyServiceProvider.CreateContact(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult DeleteContact(string contactKey)
        {
            try
            {
                return _companyServiceProvider.DeleteContact(contactKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        #endregion

        #region customer note methods

        public IResult<ICustomerCompanyNoteReturn> GetCustomerNote(string customerNoteKey)
        {
            try
            {
                return _companyServiceProvider.GetCustomerNote(customerNoteKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICustomerCompanyNoteReturn>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<string>> GetDistinctCustomerNoteTypes()
        {
            try
            {
                return _companyServiceProvider.GetDistinctCustomerNoteTypes();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<string>>(null, ex.Message);
            }
        }

        public IResult<string> CreateCustomerNote(ICreateCustomerNoteParameters parameters)
        {
            try
            {
                return _companyServiceProvider.CreateCustomerNote(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateCustomerNote(IUpdateCustomerNoteParameters parameters)
        {
            try
            {
                return _companyServiceProvider.UpdateCustomerNote(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult DeleteCustomerNote(string customerNoteKey)
        {
            try
            {
                return _companyServiceProvider.DeleteCustomerNote(customerNoteKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        #endregion
    }
}