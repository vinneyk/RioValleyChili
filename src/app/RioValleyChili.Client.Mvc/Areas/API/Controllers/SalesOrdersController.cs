using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class SalesOrdersController : ApiController
    {
        private readonly ISalesService _salesService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public SalesOrdersController(ISalesService salesService, IUserIdentityProvider userIdentityProvider)
        {
            if(salesService == null) { throw new ArgumentNullException("salesService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            
            _salesService = salesService;
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<SalesOrderSummaryResponse> Get(SalesOrderStatus? orderStatus = null, string customerKey = null, string brokerKey = null, DateTime? orderReceivedRangeStart = null, DateTime? orderReceivedRangeEnd = null, int skipCount = 0, int pageSize = 20)
        {
            var result = _salesService.GetSalesOrders(new FilterSalesOrdersParameters
                {
                    CustomerKey = customerKey,
                    BrokerKey = brokerKey,
                    SalesOrderStatus = orderStatus,
                });
            result.EnsureSuccessWithHttpResponseException();

            orderReceivedRangeStart = orderReceivedRangeStart.HasValue ? orderReceivedRangeStart.Value.Date : (DateTime?) null;
            orderReceivedRangeEnd = orderReceivedRangeEnd.HasValue ? orderReceivedRangeEnd.Value.Date.AddDays(1) : (DateTime?) null;

            var mapped = result.ResultingObject
                .Where(m =>
                    (orderStatus == null || m.SalesOrderStatus == orderStatus)
                    && (orderReceivedRangeStart == null || m.DateOrderReceived >= orderReceivedRangeStart)
                    && (orderReceivedRangeEnd == null || m.DateOrderReceived < orderReceivedRangeEnd))

                .OrderBy(m => m.Shipment.Status)
                .ThenBy(m => m.ShipmentDate)

                .PageResults(pageSize, skipCount)
                .Project().To<SalesOrderSummaryResponse>();
            return mapped;
        }

        public SalesOrderDetailsResponse Get(string id)
        {
            var result = _salesService.GetSalesOrder(id);
            result.EnsureSuccessWithHttpResponseException();
            var mapped = result.ResultingObject.Map().To<SalesOrderDetailsResponse>();
            mapped.Links = new ResourceLinkCollection
                {
                    this.BuildSelfLink(mapped.MovementKey, "self"),
                    this.BuildReportLink(mapped.IsMiscellaneous ? MVC.Reporting.SalesReporting.MiscOrderCustomerConfirmation(mapped.MovementKey) : MVC.Reporting.SalesReporting.CustomerOrderConfirmation(mapped.MovementKey), "report-customer-order-confirmation"),
                    this.BuildReportLink(mapped.IsMiscellaneous ? MVC.Reporting.SalesReporting.MiscOrderInternalConfirmation(mapped.MovementKey) : MVC.Reporting.SalesReporting.InHouseConfirmation(mapped.MovementKey), "report-in-house-confirmation"),
                    this.BuildReportLink(mapped.IsMiscellaneous ? MVC.Reporting.SalesReporting.MiscInvoice(mapped.MovementKey) : MVC.Reporting.SalesReporting.CustomerInvoice(mapped.MovementKey), "report-customer-order-invoice"),
                    this.BuildReportLink(MVC.Reporting.SalesReporting.InHouseInvoice(mapped.MovementKey), "report-in-house-invoice"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.CertificateOfAnalysis(mapped.MovementKey), "report-inv_shipment-coa"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PackingList(mapped.MovementKey), "report-inv_shipment-packing-list"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PackingListBarcode(mapped.MovementKey), "report-inv_shipment-packing-list-bar-code"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.BillOfLading(mapped.MovementKey), "report-inv_shipment-bol"),
                    this.BuildReportLink(MVC.Reporting.InventoryShipmentOrderReporting.PickSheet(mapped.MovementKey), "report-inv_shipment-pick-sheet"),
                };
            return mapped;
        }

        public HttpResponseMessage Post([FromBody]CreateSalesOrderRequest values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ModelState.GetErrorSummary())
                    };
            }

            var param = values.Map().To<CreateSalesOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);

            var result = _salesService.CreateSalesOrder(param);
            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }
        
        public HttpResponseMessage Put(string id, [FromBody]UpdateSalesOrderRequest values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ModelState.GetErrorSummary())
                    };
            }

            var param = values.Map().To<UpdateSalesOrderParameters>();
            _userIdentityProvider.SetUserIdentity(param);

            var result = _salesService.UpdateSalesOrder(param);
            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        public HttpResponseMessage Delete(string id)
        {
            return _salesService.DeleteSalesOrder(id).ToHttpResponseMessage(HttpVerbs.Delete);
        }

        [System.Web.Http.HttpPost, System.Web.Http.Route("api/SalesOrders/{id}/PostInvoice")]
        public HttpResponseMessage PostInvoice(string id)
        {
            var result = _salesService.PostInvoice(id);
            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }
    }
}
