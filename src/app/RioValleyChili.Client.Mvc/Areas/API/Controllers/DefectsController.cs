using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class DefectsController : ApiController
    {
        private readonly ILotService _lotService;

        public DefectsController(ILotService lotService)
        {
            if (lotService == null) { throw new ArgumentNullException("lotService"); }
            _lotService = lotService;
        }

        public HttpResponseMessage Post([FromBody] CreateDefectDto value)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var resolveLotDefectResult =  _lotService.CreateLotDefect(value);
            return resolveLotDefectResult
                .ToMapped().Response<CreateLotDefectResponse>(HttpVerbs.Post);
        }
    }
}