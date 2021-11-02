using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ProductionResultsController : ApiController
    {
        #region fields and constructors

        private readonly IProductionResultsService _productionResultsService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ProductionResultsController(IProductionResultsService productionResultsService, IUserIdentityProvider userIdentityProvider)
        {
            if (productionResultsService == null) { throw new ArgumentNullException("productionResultsService"); }
            _productionResultsService = productionResultsService;

            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion

        // GET api/productionresults
        public IEnumerable<string> Get()
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        // GET api/productionresults/5
        public ProductionResultDetail Get(string id)
        {
            var getBatchResult = _productionResultsService.GetProductionResultDetail(id);
            getBatchResult.EnsureSuccessWithHttpResponseException();
            return getBatchResult.ResultingObject.Map().To<ProductionResultDetail>();
        }

        // POST api/productionresults
        public HttpResponseMessage Post([FromBody]CreateProductionBatchResultsDto value)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The supplied values are not valid. Please review data for errors and retry.")
                    };
            }
            var parameters = _userIdentityProvider.SetUserIdentity(value.Map().To<CreateProductionBatchResultsParameters>());

            var saveResult = _productionResultsService.CreateProductionBatchResults(parameters);
            return saveResult.ToHttpResponseMessage(HttpVerbs.Post);
        }

        // PUT api/productionresults/5
        public HttpResponseMessage Put(string id, [FromBody]UpdateProductionBatchResultsDto value)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = ModelState.GetErrorSummary()
                    };
            }
            var parameters = _userIdentityProvider.SetUserIdentity(value.Map().To<UpdateProductionBatchResultsParameters>());
            parameters.ProductionResultKey = id;
            
            var saveResult = _productionResultsService.UpdateProductionBatchResults(parameters);
            return saveResult.ToHttpResponseMessage(HttpVerbs.Put);
        }

        // DELETE api/productionresults/5
        public void Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }
    }
}
