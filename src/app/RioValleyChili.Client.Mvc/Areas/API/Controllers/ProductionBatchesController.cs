using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.ProductionBatch),
    System.Web.Http.RoutePrefix("api/productionbatches")]
    public class ProductionBatchesController : ApiController
    {
        private readonly IProductionService _productionService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ProductionBatchesController(IProductionService productionService, IUserIdentityProvider userIdentityProvider)
        {
            if (productionService == null) { throw new ArgumentNullException("productionService"); }
            _productionService = productionService;

            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        [System.Web.Http.Route("lot/{lotKey}")]
        public ProductionBatchDetails GetBatchByLotKey(string lotKey)
        {
            return Get(lotKey);
        }

        // GET api/productionbatches/02 13 001 01
        [System.Web.Http.Route("{id}")]
        public ProductionBatchDetails Get(string id)
        {
            var result = _productionService.GetProductionBatch(id);
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject.Map().To<ProductionBatchDetails>();
        }

        // POST api/productionbatches
        [ValidateAntiForgeryTokenFromCookie, ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.ProductionBatch)]
        public HttpResponseMessage Post([FromBody]CreateProductionBatchDto value)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }
            var dto = value.Map().To<CreateProductionBatchParameters>();
            _userIdentityProvider.SetUserIdentity(dto);

            var createBatchResult = _productionService.CreateProductionBatch(dto);
            if (!createBatchResult.Success)
            {
                return createBatchResult.ToHttpResponseMessage(HttpVerbs.Post);
            }

            return new HttpResponseMessage(HttpStatusCode.Created)
                       {
                           Content = new ObjectContent<ICreateProductionBatchReturn>(createBatchResult.ResultingObject, new JsonMediaTypeFormatter())
                       };
        }

        // PUT api/productionbatches/02 13 001 01
        [System.Web.Http.Route("{id}")]
        [ValidateAntiForgeryTokenFromCookie, ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.ProductionClaimTypes.ProductionBatch)]
        public HttpResponseMessage Put(string id, [FromBody]UpdateProductionBatchParameters value)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ModelState.GetErrorSummary())
                };
            }

            value.ProductionBatchKey = id;
            _userIdentityProvider.SetUserIdentity(value);
            var result = _productionService.UpdateProductionBatch(value);
            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        // DELETE api/productionbatches/5
        [ValidateAntiForgeryTokenFromCookie, ClaimsAuthorize(ClaimActions.Delete, ClaimTypes.ProductionClaimTypes.ProductionBatch)]
        [System.Web.Http.Route("{id}")]
        public HttpResponseMessage Delete(string id)
        {
            return _productionService.RemoveProductionBatch(id).ToHttpResponseMessage(HttpVerbs.Delete);
        }
    }
}
