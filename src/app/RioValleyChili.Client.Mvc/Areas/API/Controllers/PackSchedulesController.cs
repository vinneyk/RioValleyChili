using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.PackSchedules)]
    public class PackSchedulesController : ApiController
    {
        #region fields and constructors

        private readonly IProductionService _productionService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public PackSchedulesController(IProductionService productionService, IUserIdentityProvider userIdentityProvider)
        {
            if (productionService == null) { throw new ArgumentNullException("productionService"); }
            _productionService = productionService;

            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider");}
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion

        // GET /api/packschedules
        public IEnumerable<PackScheduleSummary> Get(int skipCount = 0, int pageSize = 10, DateTime? beginningCreatedDateRange = null, DateTime? endingCreatedDateRange = null, DateTime? beginningDateScheduledRange = null, DateTime? endingDateScheduledRange = null, string lineNumberFilter = null)
        {
            var packeschedulesQueryResult = _productionService.GetPackSchedules();
            packeschedulesQueryResult.EnsureSuccessWithHttpResponseException();
            
            return packeschedulesQueryResult.ResultingObject
                .Where(p =>
                    (!beginningCreatedDateRange.HasValue || p.DateCreated >= beginningCreatedDateRange)
                    && (!endingCreatedDateRange.HasValue || p.DateCreated <= endingCreatedDateRange)
                    && (!beginningDateScheduledRange.HasValue || p.ScheduledProductionDate >= beginningDateScheduledRange)
                    && (!endingDateScheduledRange.HasValue || p.ScheduledProductionDate <= endingDateScheduledRange)
                ).OrderByDescending(p => p.DateCreated)
                .PageResults(pageSize, skipCount)
                .Project().To<PackScheduleSummary>();
        }

        // GET /api/packschedules/5
        public PackScheduleDetails Get(string id)
        {
            int psNum;
            if (int.TryParse(id, out psNum))
            {
                return FindPackScheduleByOldPackScheduleNumber(psNum);
            }
            
            var getPackScheduleSummaryResult = _productionService.GetPackSchedule(id);
            getPackScheduleSummaryResult.EnsureSuccessWithHttpResponseException();

            return getPackScheduleSummaryResult.ResultingObject.Map().To<PackScheduleDetails>();
        }

        // POST /api/packschedules
        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.PackSchedules),
        ValidateAntiForgeryTokenFromCookie]
        public HttpResponseMessage Post(CreatePackSchedule values)
        {
            if (values == null || !ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = ModelState.GetErrorSummary()
                };
            }

            var parameters = values.Map().To<CreatePackScheduleParameters>();
            _userIdentityProvider.SetUserIdentity(parameters);
            return _productionService.CreatePackSchedule(parameters).ToHttpResponseMessage(HttpVerbs.Post);
        }

        // PUT /api/packschedules/5
        [ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.ProductionClaimTypes.PackSchedules),
        ValidateAntiForgeryTokenFromCookie]
        public HttpResponseMessage Put(string id, UpdatePackScheduleParameters values)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = ModelState.GetErrorSummary()
                };
            }

            values.PackScheduleKey = id;
            var parameters = values.Map().To<UpdatePackScheduleParameters>();
            _userIdentityProvider.SetUserIdentity(parameters);
            return _productionService.UpdatePackSchedule(parameters).ToHttpResponseMessage();
        }

        // DELETE /api/packschedules/5
        [ClaimsAuthorize(ClaimActions.Delete, ClaimTypes.ProductionClaimTypes.PackSchedules),
        ValidateAntiForgeryTokenFromCookie]
        public HttpResponseMessage Delete(string id)
        {
            var dto = new DeletePackScheduleParameters
            {
                PackScheduleKey = id
            };
            _userIdentityProvider.SetUserIdentity(dto);
            return _productionService.RemovePackSchedule(dto).ToHttpResponseMessage(HttpVerbs.Delete);
        }

        internal class DeletePackScheduleParameters : IRemovePackScheduleParameters
        {
            public string UserToken { get; set; }
            public string PackScheduleKey { get; set; }
        }

        private PackScheduleDetails FindPackScheduleByOldPackScheduleNumber(int psNum)
        {
            var packScheduleSummariesResult = _productionService.GetPackSchedules();
            packScheduleSummariesResult.EnsureSuccessWithHttpResponseException();
            var psSummary = packScheduleSummariesResult.ResultingObject.FirstOrDefault(ps => ps.PSNum == psNum);
            
            // prevent recursive call which could occur if PackScheduleKey == psNum
            if (psSummary != null && psSummary.PackScheduleKey != psNum.ToString(CultureInfo.InvariantCulture))
            {
                return Get(psSummary.PackScheduleKey);
            } 
            
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}
