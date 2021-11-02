using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.RoutePrefix("~/api/products/{productKey}/specs")]
    public class ProductSpecsController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ProductSpecsController(IProductService productService, IUserIdentityProvider userIdentityProvider)
        {
            if (productService == null) { throw new ArgumentNullException("productService"); }
            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _productService = productService;
            _userIdentityProvider = userIdentityProvider;
        }

        // GET: api/products/123/specs
        public IEnumerable<IProductAttributeRangeReturn> Get(string productKey)
        {
            var getChileProductResult = _productService.GetChileProductDetail(productKey);
            getChileProductResult.EnsureSuccessWithHttpResponseException();
            return getChileProductResult.ResultingObject.AttributeRanges;
        }
        
        // POST: api/products/123/specs
        public HttpResponseMessage Post(string productKey, [FromBody]SetChileProductAttributeRangesRequest values)
        {
            if(!ModelState.IsValid) { return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ModelState.GetErrorSummary() }; }

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetChileProductAttributeRangesParameters>());
            parameters.ChileProductKey = productKey;
            var result = _productService.SetChileProductAttributeRanges(parameters);

            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }
    }
}
