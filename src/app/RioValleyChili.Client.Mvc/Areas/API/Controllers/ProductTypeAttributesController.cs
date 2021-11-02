using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ProductTypeAttributesController : ApiController
    {
        #region fields and constructors
        
        private readonly IInventoryService _inventoryService;

        public ProductTypeAttributesController(IInventoryService inventoryService)
        {
            if (inventoryService == null) { throw new ArgumentNullException("inventoryService"); }
            _inventoryService = inventoryService;
        }

        #endregion

        // GET api/productTypeAttributes
        public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> Get()
        {
            var results = _inventoryService.GetInventory();
            results.EnsureSuccessWithHttpResponseException();
            return results.ResultingObject.AttributeNamesByProductType;
        }

        // GET api/productTypeAttributes/5
        public string Get(int id)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // POST api/productTypeAttributes
        public void Post([FromBody]string value)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // PUT api/productTypeAttributes/5
        public void Put(int id, [FromBody]string value)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // DELETE api/productTypeAttributes/5
        public void Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }
    }
}
