using System;
using System.Collections.Generic;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ShipmentMethodsController : ApiController
    {
        private readonly IInventoryShipmentOrderService _shipmentOrderService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ShipmentMethodsController(IInventoryShipmentOrderService shipmentOrderService, IUserIdentityProvider userIdentityProvider)
        {
            if(shipmentOrderService == null) { throw new ArgumentNullException("shipmentOrderService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            
            _shipmentOrderService = shipmentOrderService;
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<string> Get()
        {
            var result = _shipmentOrderService.GetShipmentMethods();
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject;
        }
    }
}