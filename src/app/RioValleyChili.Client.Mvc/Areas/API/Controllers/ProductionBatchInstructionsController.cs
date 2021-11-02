using System;
using System.Collections.Generic;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ProductionBatchInstructionsController : ApiController
    {
        private readonly IProductionService _productionService;

        public ProductionBatchInstructionsController(IProductionService productionService)
        {
            if(productionService == null) throw new ArgumentNullException("productionService");
            _productionService = productionService;
        }

        public IEnumerable<string> Get()
        {
            var result = _productionService.GetProductionBatchInstructions();
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject;
        }
    }
}
