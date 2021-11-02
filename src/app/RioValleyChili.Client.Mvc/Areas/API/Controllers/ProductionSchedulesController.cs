using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ProductionSchedulesController : ApiController
    {
        private readonly IProductionService _productionService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ProductionSchedulesController(IProductionService productionService, IUserIdentityProvider userIdentityProvider)
        {
            if(productionService == null) { throw new ArgumentNullException("productionService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }

            _productionService = productionService;
            _userIdentityProvider = userIdentityProvider;
        }

        /// <summary>
        /// Returns collection of Production Schedule Summaries, optionally filtered and paged
        /// </summary>
        /// <param name="productionDate">Optional filtering by production date.</param>
        /// <param name="productionLineLocationKey">Optional filtering by production line location key; note this is *not* the line number, but the actual location key for the production line.</param>
        /// <param name="skipCount">Page skip count.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Collection of Production Schedule Summaries</returns>
        public IEnumerable<ProductionScheduleSummaryResponse> Get(DateTime? productionDate = null, string productionLineLocationKey = null,
            int? skipCount = null, int? pageSize = null)
        {
            var result = _productionService.GetProductionSchedules(new FilterProductionScheduleParameters
                {
                    ProductionDate = productionDate,
                    ProductionLineLocationKey = productionLineLocationKey
                });
            result.EnsureSuccessWithHttpResponseException();

            var response = result.ResultingObject
                .OrderByDescending(p => p.ProductionDate)
                .PageResults(pageSize, skipCount)
                .Project().To<ProductionScheduleSummaryResponse>();

            return response;
        }

        public ProductionScheduleDetailResponse Get(string id)
        {
            var result = _productionService.GetProductionSchedule(id);
            result.EnsureSuccessWithHttpResponseException();

            var response = result.ResultingObject.Map().To<ProductionScheduleDetailResponse>();
            response.Links = new ResourceLinkCollection
                {
                    this.BuildSelfLink(id, "self"),
                    this.BuildReportLink(MVC.Reporting.ProductionReporting.ProductionSchedule(result.ResultingObject.ProductionDate), "report-prod-schedule-day"),
                    this.BuildReportLink(MVC.Reporting.ProductionReporting.ProductionSchedule(result.ResultingObject.ProductionDate, result.ResultingObject.ProductionLine.LocationKey), "report-prod-schedule-day+line")
                };

            return response;
        }

        public HttpResponseMessage Post([FromBody]CreateProductionScheduleRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<CreateProductionScheduleParameters>());
            var result = _productionService.CreateProductionSchedule(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);

            return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = Get(result.ResultingObject).ToJSONContent()
                };
        }

        public HttpResponseMessage Put(string id, [FromBody]UpdateProductionScheduleRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<UpdateProductionScheduleParameters>());
            parameters.ProductionScheduleKey = id;
            var result = _productionService.UpdateProductionSchedule(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = Get(id).ToJSONContent()
                };
        }

        public HttpResponseMessage Delete(string id)
        {
            return _productionService.DeleteProductionSchedule(id).ToHttpResponseMessage(HttpVerbs.Delete);
        }
    }
}