using System;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class WorkModesController : ApiController
    {
        #region fields and constructors

        private readonly IProductionService _productionService;

        public WorkModesController(IProductionService productionService)
        {
            if (productionService == null)
            {
                throw new ArgumentNullException("productionService");
            }
            _productionService = productionService;
        }

        #endregion

        #region GET /api/workmodes

        public IQueryable<IWorkTypeReturn> Get()
        {
            var result = _productionService.GetWorkTypes();
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject;
        }

        #endregion

        #region GET /api/workmodes/5

        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST /api/workmodes

        public void Post(string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT /api/workmodes/5

        public void Put(int id, string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE /api/workmodes/5

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
