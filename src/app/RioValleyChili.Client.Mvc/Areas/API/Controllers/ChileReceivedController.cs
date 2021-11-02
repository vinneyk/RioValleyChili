using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.ChileMaterialsReceived;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Models.Parameters;
using ClaimsAuthorizeAttribute = Thinktecture.IdentityModel.Authorization.WebApi.ClaimsAuthorizeAttribute;
using CreateChileMaterialsReceivedParameters = RioValleyChili.Services.Models.Parameters.CreateChileMaterialsReceivedParameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.InventoryClaimTypes.DehydratedMaterials)]
    public class ChileReceivedController : ApiController
    {
        #region fields and constructors

        private readonly IMaterialsReceivedService _materialsReceivedService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public ChileReceivedController(IMaterialsReceivedService materialsReceivedService, IUserIdentityProvider userIdentityProvider)
        {
            if (materialsReceivedService == null) { throw new ArgumentNullException("materialsReceivedService"); }
            _materialsReceivedService = materialsReceivedService;

            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion
        
        public IEnumerable<ChileMaterialsReceivedSummaryResponse> Get(
            ChileMaterialsReceivedType? materialsType = null, string supplierKey = null, string chileProductKey = null,
            DateTime? startDate = null, DateTime? endDate = null,
            int? skipCount = null, int? pageSize = null)
        {
            var results = _materialsReceivedService.GetChileMaterialsReceivedSummaries(new ChileMaterialsReceivedFilters
                {
                    ChileMaterialsType = materialsType,
                    SupplierKey = supplierKey,
                    ChileProductKey = chileProductKey
                });
            results.EnsureSuccessWithHttpResponseException();
            var query = results.ResultingObject;

            if(startDate.HasValue)
            {
                var filterStart = startDate.Value.Date;
                query = query.Where(d => d.DateReceived >= filterStart);
            }
            if(endDate.HasValue)
            {
                var filterEnd = endDate.Value.AddDays(1).Date;
                query = query.Where(d => d.DateReceived < filterEnd);
            }

            return query.OrderByDescending(d => d.DateReceived).ThenBy(d => d.LoadNumber)
                .PageResults(pageSize, skipCount)
                .Project().To<ChileMaterialsReceivedSummaryResponse>();
        }

        public ChileMaterialsReceivedDetailResponse Get(string id)
        {
            var result = _materialsReceivedService.GetChileMaterialsReceivedDetail(id);
            result.EnsureSuccessWithHttpResponseException();

            var response = result.ResultingObject.Map().To<ChileMaterialsReceivedDetailResponse>();
            response.Links = new ResourceLinkCollection
                {
                    this.BuildReportLink(MVC.Reporting.InventoryReceivingReporting.ChileMaterialsReceivedRecap(response.LotKey), "report-recap")
                };

            return response;
        }
        
        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.InventoryClaimTypes.DehydratedMaterials)]
        public HttpResponseMessage Post([FromBody]CreateChileMaterialsReceivedRequest values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The supplied values are not valid. Please review data for errors and retry.")
                    };
            }

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<CreateChileMaterialsReceivedParameters>());
            var result = _materialsReceivedService.CreateChileMaterialsReceived(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);
            
            return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = Get(result.ResultingObject).ToJSONContent()
                };
        }
        
        [ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.InventoryClaimTypes.DehydratedMaterials)]
        public HttpResponseMessage Put(string id, [FromBody]UpdateChileMaterialsReceivedRequest values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("The supplied values are not valid. Please review data for errors and retry.")
                    };
            }

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<UpdateChileMaterialsReceivedParameters>());
            parameters.LotKey = id;
            var result = _materialsReceivedService.UpdateChileMaterialsReceived(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = Get(result.ResultingObject).ToJSONContent()
                };
        }

        public void Delete(string id)
        {
            throw new NotSupportedException("Deleting the Dehydrated Materials Received data is not currently supported.");
        }
    }
}
