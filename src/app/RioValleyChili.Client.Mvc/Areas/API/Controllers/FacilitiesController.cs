using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.Warehouses)]
    public class FacilitiesController : ApiController
    {
        #region fields and constructors

        private readonly IFacilityService _facilityService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public FacilitiesController(IFacilityService facilityService, IUserIdentityProvider userIdentityProvider)
        {
            if (facilityService == null) { throw new ArgumentNullException("facilityService"); }
            _facilityService = facilityService;

            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion

        // GET api/warehouses
        [System.Web.Http.Route("api/facilities")]
        public IEnumerable<FacilityResponse> Get()
        //public IEnumerable<Facility> Get([FromUri]bool? includeAddress) //note: the bool? param caused routing errors
        {
            //todo pass include address value into Service
            var getResult = _facilityService.GetFacilities(includeAddress: true);
            getResult.EnsureSuccessWithHttpResponseException();
            return getResult.ResultingObject.ToList()
                            .OrderBy(l => l.FacilityName)
                            .Project().To<FacilityResponse>();
        }

        // GET api/warehouses/5
        [System.Web.Http.Route("api/facilities/{id}")]
        public FacilityDetailsResponse Get(string id)
        {
            var getResult = _facilityService.GetFacilities(id, includeLocations: true, includeAddress: true);
            getResult.EnsureSuccessWithHttpResponseException();
            var value = getResult.ResultingObject
                .Project().To<FacilityDetailsResponse>().Single();

            value.Locations = value.Locations.OrderBy(l => l.GroupName).ThenBy(l => l.Row);
            return value;
        }

        // POST api/warehouses
        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.WarehouseClaimTypes.Warehouses)]
        [System.Web.Http.Route("api/facilities")]
        public HttpResponseMessage Post([FromBody]SaveFacilityParameter value)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var param = _userIdentityProvider.SetUserIdentity(value.Map().To<CreateFacilityParameters>());
            var result = _facilityService.CreateFacility(param);

            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }

        // PUT api/warehouses/5
        [ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.WarehouseClaimTypes.Warehouses)]
        [System.Web.Http.Route("api/facilities/{id}")]
        public HttpResponseMessage Put(string id, [FromBody]SaveFacilityParameter value)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var param =_userIdentityProvider.SetUserIdentity(value.Map().To<UpdateFacilityParameters>());
            param.FacilityKey = id;
            var result = _facilityService.UpdateFacility(param);

            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }
    }
}