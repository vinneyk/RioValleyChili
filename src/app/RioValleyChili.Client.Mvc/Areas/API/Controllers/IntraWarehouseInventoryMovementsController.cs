using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.RoutePrefix("api/IntraWarehouseInventoryMovements")]
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.IntrawarehouseMovements)]
    public class IntraWarehouseInventoryMovementsController : ApiController
    {
        private readonly IIntraWarehouseOrderService _intraWarehouseOrderService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public IntraWarehouseInventoryMovementsController(IIntraWarehouseOrderService intraWarehouseOrderService, IUserIdentityProvider userIdentityProvider)
        {
            if (intraWarehouseOrderService == null) { throw new ArgumentNullException("intraWarehouseOrderService"); }
            _intraWarehouseOrderService = intraWarehouseOrderService;

            if (userIdentityProvider == null) {  throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<IntraWarehouseOrderSummary> Get(int pageSize = 20, int skipCount = 0)
        {
            var results = _intraWarehouseOrderService.GetIntraWarehouseOrderSummaries();
            results.EnsureSuccessWithHttpResponseException();
            return results.ResultingObject
                .OrderByDescending(r => r.DateCreated)
                .PageResults(pageSize, skipCount)
                .Project().To<IntraWarehouseOrderSummary>();
        }

        [System.Web.Http.Route("{id}", Name = "MovementByTrackingSheet")]
        public IntraWarehouseOrderDetails Get(decimal id)
        {
            var results = _intraWarehouseOrderService.GetIntraWarehouseOrders();
            results.EnsureSuccessWithHttpResponseException();

            var result = results.ResultingObject.FirstOrDefault(o => o.TrackingSheetNumber == id);

            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result.Map().To<IntraWarehouseOrderDetails>();
        }

        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.WarehouseClaimTypes.IntrawarehouseMovements)]
        public HttpResponseMessage Post([FromBody]CreateIntraWarehouseOrder data)
        {
            if(!ModelState.IsValid) throw new HttpResponseException(HttpStatusCode.BadRequest);
            var param = data.Map().To<CreateIntraWarehouseOrderParameters>();

            _userIdentityProvider.SetUserIdentity(param);

            var results = _intraWarehouseOrderService.CreateIntraWarehouseOrder(param);
            results.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);

            return results.ToHttpResponseMessage(HttpVerbs.Post);
        }

        [System.Web.Http.Route("{id}"),
        ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.WarehouseClaimTypes.IntrawarehouseMovements)]
        public HttpResponseMessage Put(string id, [FromBody]UpdateIntraWarehouseOrder data)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }
            var param = data.Map().To<UpdateIntraWarehouseOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            param.IntraWarehouseOrderKey = id;
            var results = _intraWarehouseOrderService.UpdateIntraWarehouseOrder(param);
            return results.ToHttpResponseMessage(HttpVerbs.Put);
        }

        private KeyValuePair<string, Link> BuildIntraWarehouseMovementLink(decimal id, string rel = "intra-warehouse-movement")
        {
            return new KeyValuePair<string, Link>(rel, new Link
            {
                HRef = Url.Route("MovementByTrackingSheet", new { id })
            });
        }
    }
}
