using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.Route("api/customers/{customerKey}/productSpecs/{productKey?}"),
    ClaimsAuthorize(ClaimActions.Full, ClaimTypes.QualityControlClaimTypes.CustomerProductSpec)]
    public class CustomerProductSpecsController : ApiController
    {
        private readonly ISalesService _salesService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public CustomerProductSpecsController(ISalesService salesService, IUserIdentityProvider userIdentityProvider)
        {
            if(salesService == null) throw new ArgumentNullException("salesService");
            _salesService = salesService;

            if(userIdentityProvider == null) throw new ArgumentNullException("userIdentityProvider");
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<CustomerChileProductAttributeRangesReturn> Get(string customerKey)
        {
            var serviceResult = _salesService.GetCustomerChileProductsAttributeRanges(customerKey);
            serviceResult.EnsureSuccessWithHttpResponseException();

            return serviceResult.ResultingObject.Project().To<CustomerChileProductAttributeRangesReturn>();
        }

        public IEnumerable<CustomerChileProductAttributeRangeReturn> Get(string customerKey, string productKey)
        {
            var serviceResult = _salesService.GetCustomerChileProductAttributeRanges(customerKey, productKey);
            serviceResult.EnsureSuccessWithHttpResponseException();

            return serviceResult.ResultingObject.AttributeRanges.Project().To<CustomerChileProductAttributeRangeReturn>();
        }

        [ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Full, ClaimTypes.QualityControlClaimTypes.CustomerProductSpec)]
        public HttpResponseMessage Post(string customerKey, string productKey, [FromBody]IEnumerable<CustomerProductRangeRequest> values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = ModelState.GetErrorSummary()
                    };
            }
            var parameters = _userIdentityProvider.SetUserIdentity(new SetCustomerProductRangesRequest
                {
                    CustomerKey = customerKey,
                    ChileProductKey = productKey,
                    AttributeRanges = values
                }.Map().To<SetCustomerProductAttributeRangesParameters>());
            var result = _salesService.SetCustomerChileProductAttributeRanges(parameters);

            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }

        [ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Delete, ClaimTypes.QualityControlClaimTypes.CustomerProductSpec)]
        public HttpResponseMessage Delete(string customerKey, string productKey)
        {
            return _salesService.RemoveCustomerChileProductAttributeRanges(new RemoveCustomerChileProductAttributeRangesParameters
                {
                    CustomerKey = customerKey,
                    ChileProductKey = productKey
                }).ToHttpResponseMessage(HttpVerbs.Delete);
        }
    }
}
