using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [Obsolete("Use ProductsController instead.")]
    public class ChileProductsController : ApiController
    {
        #region fields

        private readonly IProductService _inventoryService;

        #endregion

        #region constructors

        public ChileProductsController(IProductService inventoryService)
        {
            if(inventoryService == null) { throw new ArgumentNullException("inventoryService"); }

            _inventoryService = inventoryService;
        }

        #endregion

        #region action methods

        #region  GET /api/products

        public IEnumerable<ChileProductResponse> Get(ChileStateEnum? chileState = null, bool includeInactive = false)
        {
            return _inventoryService.GetChileProducts(chileState).ResultingObject
                .Where(c => includeInactive || c.IsActive)
                .OrderBy(chile => chile.ProductName)
                .Project().To<ChileProductResponse>();
        }

        #endregion

        #region GET /api/products/5
        
        public IChileProductDetailReturn Get(string id)
        {
            var chileProduct = _inventoryService.GetChileProductDetail(id);
            if(chileProduct.State == ResultState.Invalid || chileProduct.ResultingObject == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            
            if(chileProduct.State == ResultState.Failure)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }


            return chileProduct.ResultingObject;
        }

        #endregion

        #region POST /api/products

        public void Post(string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT /api/products/5

        public void Put(int id, string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE /api/products/5

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
