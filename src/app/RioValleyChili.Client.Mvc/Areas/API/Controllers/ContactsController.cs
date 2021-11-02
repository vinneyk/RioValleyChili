using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ContactsController : ApiController
    {
        #region fields and constructors

        private readonly ICompanyService _companyService;
        private readonly IUserIdentityProvider _identityProvider;

        public ContactsController(ICompanyService companyService, IUserIdentityProvider identityProvider)
        {
            if (companyService == null) { throw new ArgumentNullException("companyService"); }
            _companyService = companyService;

            if(identityProvider == null) { throw new ArgumentNullException("identityProvider"); }
            _identityProvider = identityProvider;
        }

        #endregion

        #region api methods
        
        // GET api/contacts (eventually...maybe)
        // GET api/companies/123/contacts
        public IEnumerable<ContactSummaryResponse> Get(string companyKey)
        {
            var companyResult = _companyService.GetContacts(new FilterContactsParameters { CompanyKey = companyKey });
            companyResult.EnsureSuccessWithHttpResponseException();
            return companyResult.ResultingObject.Project().To<ContactSummaryResponse>();
        }

        // GET api/contacts/5 (eventually...maybe)
        // GET api/companies/123/contacts/5
        public ContactSummaryResponse Get(string companyKey, string id)
        {
            var contactResult = _companyService.GetContact(id);
            contactResult.EnsureSuccessWithHttpResponseException();

            if(!string.IsNullOrEmpty(contactResult.ResultingObject.CompanyKey) && contactResult.ResultingObject.CompanyKey != companyKey)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(string.Format("The company \"{0}\" does not contain a contact with the id \"{1}\".", companyKey, id))
                    });
            }

            return contactResult.ResultingObject.Map().To<ContactSummaryResponse>();
        }

        // POST api/contacts
        public HttpResponseMessage Post([FromBody]CreateContactRequest value)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The values contain some invalid data. Please check the data and try again.")
                    });
            }

            var parameters = _identityProvider.SetUserIdentity(value.Map().To<CreateContactParameters>());
            var results = _companyService.CreateContact(parameters);
            results.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);

            var getContactResult = _companyService.GetContact(results.ResultingObject);
            getContactResult.EnsureSuccessWithHttpResponseException();

            return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = getContactResult.ResultingObject.ToJSONContent()
                };
        }

        // PUT api/contacts/5
        public HttpResponseMessage Put(string id, [FromBody]UpdateContractRequest value)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The values contain some invalid data. Please check the data and try again.")
                    });
            }

            var parameters = _identityProvider.SetUserIdentity(value.Map().To<UpdateContactParameters>());
            parameters.ContactKey = id;

            var results = _companyService.UpdateContact(parameters);
            results.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            var getContactResult = _companyService.GetContact(id);
            getContactResult.EnsureSuccessWithHttpResponseException();

            return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = getContactResult.ResultingObject.ToJSONContent()
                };
        }

        // DELETE api/contacts/5
        public HttpResponseMessage Delete(string id)
        {
            var result = _companyService.DeleteContact(id);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Delete);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        #endregion
    }
}
