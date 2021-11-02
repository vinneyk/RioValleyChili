using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class CompaniesController : ApiController
    {
        public CompaniesController(ICompanyService companyService, IUserIdentityProvider identityProvider)
        {
            if(companyService == null) { throw new ArgumentNullException("companyService"); }
            _companyService = companyService;

            if(identityProvider == null) { throw new ArgumentNullException("identityProvider"); }
            _identityProvider = identityProvider;
        }

        // GET api/companies
        public IEnumerable<CompanySummaryResponse> Get(CompanyType? companyType = null, string filterBrokerKey = null, bool? includeInactive = false)
        {
            var results = _companyService.GetCompanies(new FilterCompanyParameters
                {
                    CompanyType = companyType,
                    IncludeInactive = includeInactive,
                    BrokerKey = filterBrokerKey
                });

            results.EnsureSuccessWithHttpResponseException();

            return results.ResultingObject
                .OrderBy(c => c.Name)
                .Project().To<CompanySummaryResponse>();
        }

        // GET api/companies/5
        public CompanyDetailResponse Get(string id)
        {
            var results = _companyService.GetCompany(id);
            results.EnsureSuccessWithHttpResponseException();

            var response = results.ResultingObject.Map().To<CompanyDetailResponse>();
            return response;
        }

        // POST api/companies
        public HttpResponseMessage Post([FromBody]CreateCompanyRequest values)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The company data contains some invalid data. Please correct for errors and try again.")
                    });
            }

            var parameters = _identityProvider.SetUserIdentity(values.Map().To<CreateCompanyParameters>());
            var result = _companyService.CreateCompany(parameters);

            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);

            return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = Get(result.ResultingObject).ToJSONContent()
                };
        }

        // PUT api/companies/5
        public HttpResponseMessage Put(string id, [FromBody]UpdateCompanyRequest values)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The company data contains some invalid data. Please correct for errors and try again.")
                    });
            }

            var parameters = _identityProvider.SetUserIdentity(values.Map().To<UpdateCompanyParameters>());
            parameters.CompanyKey = id;

            var result = _companyService.UpdateCompany(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = Get(id).ToJSONContent()
                };
        }

        // DELETE api/companies/5
        public HttpResponseMessage Delete(string id)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        [System.Web.Http.Route("api/companies/{customerKey}/notes"), System.Web.Http.HttpPost]
        public HttpResponseMessage PostCustomerNote(string customerKey, [FromBody]SetCustomerNoteRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _identityProvider.SetUserIdentity(values.Map().To<CreateCustomerNoteParameters>());
            parameters.CustomerKey = customerKey;
            var createResult = _companyService.CreateCustomerNote(parameters);
            createResult.EnsureSuccessWithHttpResponseException();

            var getResult = _companyService.GetCustomerNote(createResult.ResultingObject);
            getResult.EnsureSuccessWithHttpResponseException();
            return getResult.ToMapped().Response<CustomerCompanyNoteResponse>(HttpVerbs.Post);
        }

        [System.Web.Http.Route("api/companies/{customerKey}/notes/{id}"), System.Web.Http.HttpPut]
        public HttpResponseMessage PutCustomerNote(string customerKey, string id, [FromBody]SetCustomerNoteRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _identityProvider.SetUserIdentity(values.Map().To<UpdateCustomerNoteParameters>());
            parameters.CustomerNoteKey = id;
            var result = _companyService.UpdateCustomerNote(parameters);
            result.EnsureSuccessWithHttpResponseException();

            var getResult = _companyService.GetCustomerNote(id);
            getResult.EnsureSuccessWithHttpResponseException();
            return getResult.ToMapped().Response<CustomerCompanyNoteResponse>(HttpVerbs.Put);
        }

        [System.Web.Http.Route("api/companies/{customerKey}/notes/{id}"), System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteCustomerNote(string customerKey, string id)
        {
            var result = _companyService.DeleteCustomerNote(id);
            result.EnsureSuccessWithHttpResponseException();

            return result.ToHttpResponseMessage(HttpVerbs.Delete);
        }

        [System.Web.Http.Route("api/profilenotes/types"), System.Web.Http.HttpGet]
        public HttpResponseMessage GetNoteTypes()
        {
            var result = _companyService.GetDistinctCustomerNoteTypes();
            result.EnsureSuccessWithHttpResponseException();
            return result.ToHttpResponseMessage();
        }

        private readonly ICompanyService _companyService;
        private readonly IUserIdentityProvider _identityProvider;
    }

}