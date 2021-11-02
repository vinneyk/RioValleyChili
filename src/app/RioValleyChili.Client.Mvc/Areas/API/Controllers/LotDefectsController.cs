using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class LotDefectsController : ApiController
    {
        #region fields and constructors

        private readonly ILotService _lotService;

        public LotDefectsController(ILotService lotService)
        {
            if (lotService == null) { throw new ArgumentNullException("lotService"); }
            _lotService = lotService;
        }

        #endregion

        #region api methods

        // GET api/lots/03 13 001 01/defects
        public IEnumerable<ILotDefectReturn> Get(string lotKey)
        {
            var lotResults = _lotService.GetLotSummary(lotKey);
            lotResults.EnsureSuccessWithHttpResponseException();
            return lotResults.ResultingObject.LotSummary.Defects;
        }

        // GET api/lots/03 13 001 01/defects/20090726-11-2-1
        public ILotDefectReturn Get(string lotKey, string id)
        {
            var defects = Get(lotKey);
            var defect = defects.FirstOrDefault(d => d.LotDefectKey == id);
            if (defect == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return defect;
        }

        // GET api/lots/03 13 001 01/defects
        public HttpResponseMessage Post([FromBody] CreateLotDefectDto value)
        {
            var result = _lotService.CreateLotDefect(value);
            var message = result.ToMapped().Response<CreateLotDefectResponse>(HttpVerbs.Post);
            return message;
        }

        // GET api/lots/03 13 001 01/defects/20090726-11-2-1
        public void Put(int id, [FromBody] string value)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // GET api/lots/03 13 001 01/defects/20090726-11-2-1
        public void Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        #endregion
    }
}
