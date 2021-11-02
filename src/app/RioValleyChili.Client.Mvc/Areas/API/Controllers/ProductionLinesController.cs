using System;
using System.Collections.Generic;
using System.Web.Http;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ProductionLinesController : ApiController
    {
        #region fields

        private readonly IProductionService _productionService;

        #endregion

        #region constructors

        public ProductionLinesController(IProductionService productionService)
        {
            if(productionService == null){ throw new ArgumentNullException("productionService"); }

            _productionService = productionService;
        }

        #endregion

        #region GET /api/productionlines

        public IEnumerable<FacilityLocationResponse> Get()
        {
            var productionLinesQuery = _productionService.GetProductionLines();
            productionLinesQuery.EnsureSuccessWithHttpResponseException();
            return productionLinesQuery.ResultingObject
                .Project().To<FacilityLocationResponse>();
        }

        #endregion
    }
}
