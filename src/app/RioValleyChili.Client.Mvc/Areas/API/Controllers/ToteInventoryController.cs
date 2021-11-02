using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ToteInventoryController : ApiController
    {
        #region fields and constructors

        private readonly IInventoryService _inventoryService;

        public ToteInventoryController(IInventoryService inventoryService)
        {
            if (inventoryService == null) { throw new ArgumentNullException("inventoryService"); }
            _inventoryService = inventoryService;
        }

        #endregion
        
        // GET api/toteinventory/{toteKey}
        public ToteInventory Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            var inventoryFilter = new FilterInventoryParameters
                                      {
                                          ToteKey = id
                                      };
            var getInventoryResult = _inventoryService.GetInventory(inventoryFilter);
            getInventoryResult.EnsureSuccessWithHttpResponseException();

            var inventorySummaries = getInventoryResult.ResultingObject.Inventory.ToList();
            if (!inventorySummaries.Any()) { throw new HttpResponseException(HttpStatusCode.NotFound); }

            return new ToteInventory
                       {
                           LotKey = inventorySummaries.First().LotKey,
                           Inventory = inventorySummaries.Project().To<InventoryItem>(),
                           ToteKey = inventorySummaries.First().ToteKey,
                           Product = inventorySummaries.Select(s => s.LotProduct).Project().To<InventoryProductResponse>().Single()
                       };
        }

        // POST api/toteinventory
        public void Post([FromBody]string value)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // PUT api/toteinventory/5
        public void Put(int id, [FromBody]string value)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // DELETE api/toteinventory/5
        public void Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }
    }
}
