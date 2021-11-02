using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.TreatmentOrders;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Models.Parameters;
using Solutionhead.Services;
using CreateTreatmentOrderParameters = RioValleyChili.Services.Models.Parameters.CreateTreatmentOrderParameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.TreatmentOrders)]
    public class TreatmentOrdersController : ApiController
    {
        private readonly ITreatmentOrderService _treatmentOrderService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public TreatmentOrdersController(ITreatmentOrderService warehouseOrderService, IUserIdentityProvider userIdentityProvider)
        {
            if(warehouseOrderService == null) { throw new ArgumentNullException("warehouseOrderService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }

            _treatmentOrderService = warehouseOrderService;
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<TreatmentOrderSummary> Get(int pageSize = 20, int skipCount = 0, string originFacilityKeyFilter = null, string destinationFacilityKeyFilter = null, 
            DateTime? beginningShipmentDateFilter = null, DateTime? endingShipmentDateFilter = null, OrderStatus? orderStatusFilter = null, ShipmentStatus? shipmentStatusFilter = null)
        {
            var result = _treatmentOrderService.GetTreatmentOrders();
            result.EnsureSuccessWithHttpResponseException();

            var beginningDate = beginningShipmentDateFilter.HasValue ? (DateTime?)beginningShipmentDateFilter.Value.Date : null;
            var endingDate = endingShipmentDateFilter.HasValue ? (DateTime?)endingShipmentDateFilter.Value.Date.AddDays(1) : null;

            var mapped = result.ResultingObject
                .Where(m =>
                (
                    orderStatusFilter == null || m.OrderStatus == orderStatusFilter)
                    && (shipmentStatusFilter == null || m.Shipment.Status == shipmentStatusFilter)
                    && (beginningDate == null || m.ShipmentDate >= beginningDate)
                    && (endingDate == null || m.ShipmentDate < endingDate)
                )
                .OrderBy(m => m.OrderStatus)
                .ThenByDescending(m => m.Shipment.Status)
                .ThenByDescending(m => m.ShipmentDate)
                .PageResults(pageSize, skipCount)
                .Project().To<TreatmentOrderSummary>();

            return mapped;
        }

        public TreatmentOrderDetail Get(string id)
        {
            while(true)
            {
                var result = _treatmentOrderService.GetTreatmentOrder(id);
                if(result.State == ResultState.Invalid)
                {
                    var treatmentOrdersQuery = _treatmentOrderService.GetTreatmentOrders();
                    int moveNum;
                    if (treatmentOrdersQuery.Success && int.TryParse(id.Replace("-", null), out moveNum))
                    {
                        var moveNumResult = treatmentOrdersQuery.ResultingObject.FirstOrDefault(o => o.MoveNum == moveNum);
                        if (moveNumResult != null && moveNumResult.MovementKey != id)
                        {
                            id = moveNumResult.MovementKey;
                            continue;
                        }
                    }
                }

                result.EnsureSuccessWithHttpResponseException();
                var mapped = result.ResultingObject.Map().To<TreatmentOrderDetail>();
                mapped.Links = new ResourceLinkCollection
                {
                    this.BuildSelfLink(id, "self"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.WarehouseOrderAcknowledgement(mapped.MovementKey), "report-wh-acknowledgement"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PickSheet(mapped.MovementKey), "report-pick-list"),                    
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.BillOfLading(mapped.MovementKey), "report-bol"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PackingList(mapped.MovementKey), "report-packing-list"),
                };

                return mapped;
            }
        }

        public string Post([FromBody] CreateTreatmentOrderRequestParameter data)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var param = data.Map().To<CreateTreatmentOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            var result = _treatmentOrderService.CreateInventoryTreatmentOrder(param);

            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);
            return result.ResultingObject;
        }

        public void Put(string id, [FromBody] UpdateTreatmentOrderRequestParameter data)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var param = data.Map().To<UpdateTreatmentOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            param.TreatmentOrderKey = id;

            var result = _treatmentOrderService.UpdateTreatmentOrder(param);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }

        [System.Web.Http.Route("api/TreatmentOrders/{id}/Receive", Name = "ReceiveTreatmentOrder"), System.Web.Http.HttpPost]
        public void ReceiveTreatmentOrder(string id, [FromBody] ReceiveTreatmentOrderRequestParameter data)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var param = data.Map().To<ReceiveTreatmentOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);
            param.TreatmentOrderKey = id;

            var result = _treatmentOrderService.ReceiveOrder(param);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }

        [ValidateAntiForgeryTokenFromCookie, ClaimsAuthorize(ClaimActions.Delete, ClaimTypes.ProductionClaimTypes.ProductionBatch)]
        public HttpResponseMessage Delete(string id)
        {
            return _treatmentOrderService.DeleteTreatmentOrder(id).ToHttpResponseMessage(HttpVerbs.Delete);
        }
    }
}