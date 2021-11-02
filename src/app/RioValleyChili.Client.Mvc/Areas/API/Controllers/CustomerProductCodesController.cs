using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.CustomerProductCodes;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class CustomerProductCodesController : ApiController
    {
        private readonly ISalesService _salesService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public CustomerProductCodesController(ISalesService salesService, IUserIdentityProvider userIdentityProvider)
        {
            if(salesService == null) throw new ArgumentNullException("salesService");
            _salesService = salesService;

            if(userIdentityProvider == null) throw new ArgumentNullException("userIdentityProvider");
            _userIdentityProvider = userIdentityProvider;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customers/{customerKey}/products/{chileProductKey}/code")]
        public CustomerProductCodeResponse Get(string customerKey, string chileProductKey)
        {
            var result = _salesService.GetCustomerProductCode(customerKey, chileProductKey);
            if(!result.Success)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var mapped = result.ResultingObject.Map().To<CustomerProductCodeResponse>();
            return mapped;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/customers/{customerKey}/products/{chileProductKey}/code")]
        public HttpResponseMessage Post(string customerKey, string chileProductKey, [FromBody]CustomerProductCodeParameters data)
        {
            var result = _salesService.SetCustomerProductCode(customerKey, chileProductKey, data == null ? null : data.Value);
            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }
    }
}