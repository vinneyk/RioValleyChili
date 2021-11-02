using System;
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
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.RoutePrefix("api/products/{productKey}/ingredients")]
    public class ProductIngredientsController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ProductIngredientsController(IProductService productService, IUserIdentityProvider userIdentityProvider)
        {
            if (productService == null) { throw new ArgumentNullException("productService"); }
            _productService = productService;

            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #region api methods

        // POST api/products/1/ingredients
        public HttpResponseMessage Post(string productKey, [FromBody] SetChileProductIngredientsRequest values)
        {
            if(!ModelState.IsValid) { return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ModelState.GetErrorSummary() };}

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetChileProductIngredientsParameters>());
            parameters.ChileProductKey = productKey;
            var result = _productService.SetChileProductIngredients(parameters);

            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }
        
        #endregion
    }
}
