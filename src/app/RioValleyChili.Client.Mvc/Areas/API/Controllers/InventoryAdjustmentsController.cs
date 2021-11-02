using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using System.Collections.Generic;
using ClaimsAuthorizeAttribute = Thinktecture.IdentityModel.Authorization.WebApi.ClaimsAuthorizeAttribute;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.InventoryClaimTypes.InventoryAdjustments)]
    public class InventoryAdjustmentsController : ApiController
    {
        #region fields and constructors

        private readonly IInventoryAdjustmentsService _inventoryAdjustmentsService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public InventoryAdjustmentsController(IInventoryAdjustmentsService inventoryAdjustmentsService, IUserIdentityProvider userIdentityProvider)
        {
            if (inventoryAdjustmentsService == null) { throw new ArgumentNullException("inventoryAdjustmentsService"); }
            _inventoryAdjustmentsService = inventoryAdjustmentsService;

            if (userIdentityProvider == null) {  throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }
        #endregion

        // GET api/inventoryadjustments
        public IEnumerable<InventoryAdjustment> Get(int? skipCount = null, int? pageSize = null, string lotKey = null, DateTime? beginningDateFilter = null, DateTime? endingDateFilter = null)
        {
            var result = _inventoryAdjustmentsService.GetInventoryAdjustments(new FilterInventoryAdjustmentParameters
            {
                AdjustmentDateRangeStart = beginningDateFilter,
                AdjustmentDateRangeEnd = endingDateFilter ?? beginningDateFilter,
                LotKey = lotKey
            });

            result.EnsureSuccessWithHttpResponseException();

            return result.ResultingObject.OrderByDescending(r => r.AdjustmentDate)
                .PageResults(pageSize, skipCount)
                .Project().To<InventoryAdjustment>();
        }

        // GET api/inventoryadjustments/5
        public async Task<InventoryAdjustment> Get(string id)
        {
            var getAdjustmentResult = await Task.Run(() => _inventoryAdjustmentsService.GetInventoryAdjustment(id));
            getAdjustmentResult.EnsureSuccessWithHttpResponseException();
            return getAdjustmentResult.ResultingObject.Map().To<InventoryAdjustment>();
        }

        // POST api/inventoryadjustments
        [ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Create, ClaimTypes.InventoryClaimTypes.InventoryAdjustments)]
        public async Task<HttpResponseMessage> Post([FromBody]InventoryAdjustmentDto value)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _userIdentityProvider.SetUserIdentity(value);

            var result = _inventoryAdjustmentsService.CreateInventoryAdjustment(value);
            var httpResponse = result.ToHttpResponseMessage(HttpVerbs.Post);

            if (!result.Success) return httpResponse;

            try
            {
                var getAdjustmentResult = await Get(result.ResultingObject);
                httpResponse.Content = new ObjectContent<InventoryAdjustment>(getAdjustmentResult, new JsonMediaTypeFormatter());
            }
            catch { }

            return httpResponse;
        }

        #region Not Allowed Methods

        // PUT api/inventoryadjustments/5
        public void Put(int id, [FromBody] string value)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        // DELETE api/inventoryadjustments/5
        public void Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
        }

        #endregion

    }
}
