using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Models.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.InterwarehouseMovements)]
    public class InterWarehouseInventoryMovementsController : ApiController
    {
        #region fields and constructors
        
        private readonly IWarehouseOrderService _warehouseOrderService;
        private readonly IUserIdentityProvider _userIdentityProvider;
            
        public InterWarehouseInventoryMovementsController(IWarehouseOrderService warehouseOrderService, IUserIdentityProvider userIdentityProvider)
        {
            if(warehouseOrderService == null) { throw new ArgumentNullException("warehouseOrderService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }

            _warehouseOrderService = warehouseOrderService;
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion

        #region API methods

        public IEnumerable<InterWarehouseOrderSummary> Get(int pageSize = 20, int skipCount = 0, string originFacilityKeyFilter = null, string destinationFacilityKeyFilter = null, 
            DateTime? beginningShipmentDateFilter = null, DateTime? endingShipmentDateFilter = null, OrderStatus? orderStatusFilter = null, ShipmentStatus? shipmentStatusFilter = null)
        {
            var result = _warehouseOrderService.GetWarehouseOrders(new FilterInterWarehouseOrderParameters
                {
                    DestinationFacilityKey = destinationFacilityKeyFilter,
                    OriginFacilityKey = originFacilityKeyFilter,
                });
            result.EnsureSuccessWithHttpResponseException();

            var beginningDate = beginningShipmentDateFilter.HasValue ? (DateTime?)beginningShipmentDateFilter.Value.Date : null;
            var endingDate = endingShipmentDateFilter.HasValue ? (DateTime?)endingShipmentDateFilter.Value.Date.AddDays(1) : null;

            return result.ResultingObject
                .Where(m => 
                    (orderStatusFilter == null || m.OrderStatus == orderStatusFilter)
                    && (shipmentStatusFilter == null || m.Shipment.Status == shipmentStatusFilter)
                    && (beginningDate == null || m.ShipmentDate >= beginningDate)
                    && (endingDate == null || m.ShipmentDate < endingDate))
                .OrderByDescending(m => m.ShipmentDate)
                .PageResults(pageSize, skipCount)
                .Project().To<InterWarehouseOrderSummary>();
        }

        public InterWarehouseOrderDetails Get(string id)
        {
            while (true)
            {
                var result = _warehouseOrderService.GetWarehouseOrder(id);
                if (result.State == ResultState.Invalid)
                {
                    var warehouseOrdersQuery = _warehouseOrderService.GetWarehouseOrders();
                    int moveNum;
                    if (warehouseOrdersQuery.Success && int.TryParse(id.Replace("-", null), out moveNum))
                    {
                        var moveNumResult = warehouseOrdersQuery.ResultingObject.FirstOrDefault(w => w.MoveNum == moveNum);
                        if (moveNumResult != null && moveNumResult.MovementKey != id)
                        {
                            id = moveNumResult.MovementKey;
                            continue;
                        }
                    }
                }
                result.EnsureSuccessWithHttpResponseException();
                var response = result.ResultingObject.Map().To<InterWarehouseOrderDetails>();
                response.Links = new ResourceLinkCollection
                {
                    this.BuildSelfLink(id, "self"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.WarehouseOrderAcknowledgement(response.MovementKey), "report-wh-acknowledgement"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PickSheet(response.MovementKey), "report-pick-list"),                    
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.BillOfLading(response.MovementKey), "report-bol"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PackingList(response.MovementKey), "report-packing-list"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.CertificateOfAnalysis(response.MovementKey), "report-coa"),
                };

                return response;
            }
        }

        public string Post([FromBody]SetInventoryShipmentOrderParameters data)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var param = data.Map().To<SetOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            var result = _warehouseOrderService.CreateWarehouseOrder(param);

            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);
            return result.ResultingObject;
        }

        public void Put(string id, [FromBody]SetInventoryShipmentOrderParameters data)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var param = data.Map().To<UpdateInterWarehouseOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            param.InventoryShipmentOrderKey = id;
            param.SetShipmentInformation.InventoryShipmentOrderKey = id;

            var result = _warehouseOrderService.UpdateInterWarehouseOrder(param);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }
    
        #endregion
    }
}
