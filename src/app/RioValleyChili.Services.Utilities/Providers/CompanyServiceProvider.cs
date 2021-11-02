using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Company;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class CompanyServiceProvider : IUnitOfWorkContainer<ICompanyUnitOfWork>
    {
        public CompanyServiceProvider(ICompanyUnitOfWork companyUnitOfWork, ITimeStamper timeStamper)
        {
            if(companyUnitOfWork == null) { throw new ArgumentNullException("companyUnitOfWork"); }
            _companyUnitOfWork = companyUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        [SynchronizeOldContext(NewContextMethod.SyncCompany)]
        public IResult<string> CreateCompany(ICreateCompanyParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult.ConvertTo<string>();
            }

            var createResult = new CreateCompanyCommand(_companyUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParametersResult.ResultingObject);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _companyUnitOfWork.Commit();

            var companyKey = createResult.ResultingObject.ToCompanyKey();
            return SyncParameters.Using(new SuccessResult<string>(companyKey), companyKey);
        }

        [SynchronizeOldContext(NewContextMethod.SyncCompany)]
        public IResult UpdateCompany(IUpdateCompanyParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult.ConvertTo<string>();
            }

            var updateResult = new UpdateCompanyCommand(_companyUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParametersResult.ResultingObject);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            _companyUnitOfWork.Commit();

            var companyKey = parsedParametersResult.ResultingObject.CompanyKey;
            return SyncParameters.Using(new SuccessResult<string>(companyKey), companyKey);
        }

        public IResult<ICompanyDetailReturn> GetCompany(string companyKey)
        {
            if(companyKey == null) { throw new ArgumentNullException("companyKey"); }

            var companyKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(companyKey);
            if(!companyKeyResult.Success)
            {
                return companyKeyResult.ConvertTo<ICompanyDetailReturn>();
            }
            var parsedCompanyKey = companyKeyResult.ResultingObject.ToCompanyKey();

            var companyResult = _companyUnitOfWork.CompanyRepository
                .Filter(parsedCompanyKey.FindByPredicate)
                .SplitSelect(CompanyProjectors.SplitSelectDetail())
                .FirstOrDefault();
            if(companyResult == null)
            {
                return new InvalidResult<ICompanyDetailReturn>(null, string.Format(UserMessages.CompanyNotFound, companyKey));
            }

            return new SuccessResult<ICompanyDetailReturn>(companyResult);
        }

        public IResult<IQueryable<ICompanySummaryReturn>> GetCompanies(FilterCompanyParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<IQueryable<ICompanySummaryReturn>>();
            }

            var predicateResult = CompanyPredicateBuilder.BuildPredicate(parsedParameters.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<ICompanySummaryReturn>>();
            }

            var results = _companyUnitOfWork.CompanyRepository
                .Filter(predicateResult.ResultingObject)
                .SplitSelect(CompanyProjectors.SplitSelectSummary());
            return new SuccessResult<IQueryable<ICompanySummaryReturn>>(results);
        }

        public IResult<IQueryable<IContactSummaryReturn>> GetContacts(FilterContactsParameters parameters)
        {
            var parsedResult = parameters.ToParsedParameters();
            if(!parsedResult.Success)
            {
                return parsedResult.ConvertTo<IQueryable<IContactSummaryReturn>>();
            }

            var predicateResult = ContactPredicateBuilder.BuildPredicate(parsedResult.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<IContactSummaryReturn>>();
            }

            var selector = ContactProjectors.SelectSummary();
            var query = _companyUnitOfWork.ContactRepository.Filter(predicateResult.ResultingObject).AsExpandable().Select(selector);

            return new SuccessResult<IQueryable<IContactSummaryReturn>>(query);
        }

        public IResult<IContactSummaryReturn> GetContact(string contactKey)
        {
            if(contactKey == null) { throw new ArgumentNullException("contactKey"); }

            var contactKeyResult = KeyParserHelper.ParseResult<IContactKey>(contactKey);
            if(!contactKeyResult.Success)
            {
                return contactKeyResult.ConvertTo<IContactSummaryReturn>();
            }

            var predicate = new ContactKey(contactKeyResult.ResultingObject).FindByPredicate;
            var selector = ContactProjectors.SelectSummary();

            var contact = _companyUnitOfWork.ContactRepository.Filter(predicate).AsExpandable().Select(selector).SingleOrDefault();
            if(contact == null)
            {
                return new InvalidResult<IContactSummaryReturn>(null, string.Format(UserMessages.ContactNotFound, contactKey));
            }

            return new SuccessResult<IContactSummaryReturn>(contact);
        }

        [SynchronizeOldContext(NewContextMethod.SyncCompany)]
        public IResult<string> CreateContact(ICreateContactParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResults = parameters.ToParsedParameters();
            if(!parametersResults.Success)
            {
                return parametersResults.ConvertTo<string>();
            }

            var createResult = new CreateContactCommand(_companyUnitOfWork).Execute(parametersResults.ResultingObject);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _companyUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(createResult.ResultingObject.ToContactKey()), createResult.ResultingObject.ToCompanyKey());
        }

        [SynchronizeOldContext(NewContextMethod.SyncCompany)]
        public IResult UpdateContact(IUpdateContactParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult;
            }

            var commandResult = new UpdateContactCommand(_companyUnitOfWork).Execute(parametersResult.ResultingObject);
            if(!commandResult.Success)
            {
                return commandResult;
            }

            _companyUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), commandResult.ResultingObject.ToCompanyKey());
        }

        [SynchronizeOldContext(NewContextMethod.DeleteCompanyContact)]
        public IResult DeleteContact(string contactKey)
        {
            if(contactKey == null) { throw new ArgumentNullException("contactKey"); }

            var keyResult = KeyParserHelper.ParseResult<IContactKey>(contactKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }
            var key = keyResult.ResultingObject.ToContactKey();
            
            var contact = _companyUnitOfWork.ContactRepository.FindByKey(key, c => c.Addresses);
            if(contact == null)
            {
                return new InvalidResult(string.Format(UserMessages.ContactNotFound, contactKey));
            }
            
            var addresses = contact.Addresses.ToList();
            var tblContacts = addresses
                .Select(a =>
                    {
                        var id = a.OldContextID;
                        _companyUnitOfWork.ContactAddressRepository.Remove(a);
                        return id;
                    })
                .Concat(new [] { contact.OldContextID })
                .ToList();
            _companyUnitOfWork.ContactRepository.Remove(contact);
            _companyUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), tblContacts);
        }

        public IResult<ICustomerCompanyNoteReturn> GetCustomerNote(string customerNoteKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ICustomerNoteKey>(customerNoteKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<ICustomerCompanyNoteReturn>();
            }

            var result = _companyUnitOfWork.CustomerNoteRepository
                .Filter(keyResult.ResultingObject.ToCustomerNoteKey().FindByPredicate)
                .Select(CustomerNoteProjectors.SelectCustomerCompanyNote())
                .FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<ICustomerCompanyNoteReturn>(null, string.Format(UserMessages.CustomerNoteNotFound, customerNoteKey));
            }

            return new SuccessResult<ICustomerCompanyNoteReturn>(result);
        }

        [SynchronizeOldContext(NewContextMethod.SyncCompany)]
        public IResult<string> CreateCustomerNote(ICreateCustomerNoteParameters parameters)
        {
            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo<string>();
            }

            var result = new CustomerNoteConductor(_companyUnitOfWork).Create(customerKeyResult.ResultingObject, _timeStamper.CurrentTimeStamp, parameters);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _companyUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(result.ResultingObject.ToCustomerNoteKey()), new CompanyKey(result.ResultingObject.ToCustomerKey()));
        }

        [SynchronizeOldContext(NewContextMethod.SyncCompany)]
        public IResult UpdateCustomerNote(IUpdateCustomerNoteParameters parameters)
        {
            var customerNoteKeyResult = KeyParserHelper.ParseResult<ICustomerNoteKey>(parameters.CustomerNoteKey);
            if(!customerNoteKeyResult.Success)
            {
                return customerNoteKeyResult;
            }

            var result = new CustomerNoteConductor(_companyUnitOfWork).Update(customerNoteKeyResult.ResultingObject.ToCustomerNoteKey(), _timeStamper.CurrentTimeStamp, parameters);
            if(!result.Success)
            {
                return result;
            }

            _companyUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(customerNoteKeyResult.ResultingObject.ToCustomerNoteKey()), new CompanyKey(customerNoteKeyResult.ResultingObject.ToCustomerKey()));
        }

        [SynchronizeOldContext(NewContextMethod.DeleteCustomerNote)]
        public IResult DeleteCustomerNote(string customerNoteKey)
        {
            var customerNoteKeyResult = KeyParserHelper.ParseResult<ICustomerNoteKey>(customerNoteKey);
            if(!customerNoteKeyResult.Success)
            {
                return customerNoteKeyResult;
            }

            var customerNote = _companyUnitOfWork.CustomerNoteRepository.FindByKey(customerNoteKeyResult.ResultingObject.ToCustomerNoteKey());
            if(customerNote == null)
            {
                return new InvalidResult(string.Format(UserMessages.CustomerNoteNotFound, customerNoteKey));
            }

            var tblProfileToRemove = customerNote.EntryDate;
            _companyUnitOfWork.CustomerNoteRepository.Remove(customerNote);
            _companyUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), tblProfileToRemove);
        }

        private readonly ICompanyUnitOfWork _companyUnitOfWork;
        private readonly ITimeStamper _timeStamper;
        ICompanyUnitOfWork IUnitOfWorkContainer<ICompanyUnitOfWork>.UnitOfWork { get { return _companyUnitOfWork; } }

        public IResult<IEnumerable<string>> GetDistinctCustomerNoteTypes()
        {
            var noteTypes = _companyUnitOfWork.CustomerNoteRepository.SourceQuery.Select(n => n.Type).Distinct().ToList()
                .Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            return new SuccessResult<IEnumerable<string>>(noteTypes);
        }
    }
}