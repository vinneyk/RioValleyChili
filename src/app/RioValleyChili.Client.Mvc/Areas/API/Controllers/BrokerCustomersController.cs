using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class BrokerCustomersController : ApiController
    {
        #region fields and constructors 

        private readonly ISalesService _salesService;
        private readonly ICompanyService _companyService;

        public BrokerCustomersController(ISalesService salesService, ICompanyService companyService)
        {
            if (salesService == null)
            {
                throw new ArgumentNullException("salesService");
            }
            _salesService = salesService;

            if(companyService == null) { throw new ArgumentNullException("companyService"); }
            _companyService = companyService;
        }

        #endregion

        #region api methods

        // GET api/brokers/5/customers
        public IEnumerable<ICompanySummaryReturn> Get(string brokerKey)
        {
            return GetBroker(brokerKey).CustomerSummaries;
        }

        // GET api/brokers/5/customers/1
        public ICompanySummaryReturn Get(string brokerKey, string id)
        {
            var broker = GetBroker(brokerKey);
            var customer = broker.CustomerSummaries.FirstOrDefault(c => c.CompanyKey == id);
            if (customer == null)
            {
                var messsage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                messsage.Content = new StringContent(string.Format("The broker does not contain a company with the key \"{0}\".", id));
                throw new HttpResponseException(messsage);
            }

            return customer;
        }

        // POST api/brokers/5/customers
        public HttpResponseMessage Post(string brokerKey, [FromBody] string value)
        {
            var result = _salesService.AssignCustomerToBroker(brokerKey, value);
            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }

        // PUT api/brokers/5/customers
        public HttpResponseMessage Put(string brokerKey, [FromBody] string value)
        {
            var result = _salesService.AssignCustomerToBroker(brokerKey, value);
            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        // DELETE api/brokers/5/customers/1
        public HttpResponseMessage Delete(string brokerKey, string id)
        {
            return _salesService.RemoveCustomerFromBroker(brokerKey, id).ToHttpResponseMessage(HttpVerbs.Delete);
        }

        #endregion

        #region private methods

        private IBrokerDetailReturn GetBroker(string brokerKey)
        {
            var companyResult = _companyService.GetCompany(brokerKey);
            companyResult.EnsureSuccessWithHttpResponseException();

            var broker = companyResult.ResultingObject as IBrokerDetailReturn;
            if (broker == null)
            {
                var message = new HttpResponseMessage(HttpStatusCode.BadRequest);
                message.Content = new StringContent(string.Format("The company with key \"{0}\" was not a broker.", brokerKey));
                throw new HttpResponseException(message);
            }
            return broker;
        }

        #endregion
    }
}