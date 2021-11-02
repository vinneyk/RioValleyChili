using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class InventoryShipmentOrdersController : ApiController
    {
        #region fields and constructors
        
        private readonly IInventoryShipmentOrderService _inventoryShipmentOrderService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public InventoryShipmentOrdersController(IInventoryShipmentOrderService inventoryShipmentOrderService, IUserIdentityProvider userIdentityProvider)
        {
            if(inventoryShipmentOrderService == null) { throw new ArgumentNullException("inventoryShipmentOrderService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }

            _inventoryShipmentOrderService = inventoryShipmentOrderService;
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion

        #region API methods
        
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/InventoryShipmentOrders/{id}/PostAndClose", Name = "PostAndCloseOrder")]
        public void PostAndClose(string id, [FromBody] PostAndCloseShipmentOrderRequestParameter data)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var param = data.Map().To<PostParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            param.OrderKey = id;

            var result = _inventoryShipmentOrderService.Post(param);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }
    
        #endregion
    }
}