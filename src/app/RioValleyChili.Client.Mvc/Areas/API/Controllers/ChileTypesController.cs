using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ChileTypesController : ApiController
    {
        private readonly IProductService _inventoryService;

        public ChileTypesController(IProductService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET /api/chiletypes
        public IEnumerable<IChileTypeSummaryReturn> Get()
        {
            return _inventoryService.GetChileTypeSummaries().ResultingObject
                .OrderBy(chileType => chileType.ChileTypeDescription);
        }

        // GET /api/chiletypes/5
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST /api/chiletypes
        public void Post(string value)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
        }

        // PUT /api/chiletypes/5
        public void Put(int id, string value)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
        }

        // DELETE /api/chiletypes/5
        public void Delete(int id)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
        }
    }
}
