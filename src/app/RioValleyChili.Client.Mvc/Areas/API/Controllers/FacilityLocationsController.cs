using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Warehouse;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class FacilityLocationsController : ApiController
    {
        private readonly IFacilityService _facilityService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public FacilityLocationsController(IFacilityService facilityService, IUserIdentityProvider userIdentityProvider)
        {
            if (facilityService == null) { throw new ArgumentNullException("facilityService"); }
            _facilityService = facilityService;

            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }
        
        [OutputCache(Duration = 600, VaryByParam = "*")]
        [System.Web.Http.Route("api/facilities/{facilityKey}/locations")]
        public IEnumerable<FacilityLocationResponse> Get(string facilityKey = null)
        {
            var getResult = _facilityService.GetFacilities(facilityKey, includeLocations: true);
            getResult.EnsureSuccessWithHttpResponseException();
            return getResult.ResultingObject
                .SelectMany(r => r.Locations)
                .Project().To<FacilityLocationResponse>()
                .OrderBy(l => l.GroupName).ThenBy(l => l.Row);
        }
        
        [System.Web.Http.Route("api/facilityLocations/{id}")]
        public HttpResponseMessage Put(string id, [FromBody]UpdateLocationParameter values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var input = values.Map().To<UpdateLocationParameters>();
            input.LocationKey = id;
            _userIdentityProvider.SetUserIdentity(input);
            var result = _facilityService.UpdateLocation(input);

            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        [System.Web.Http.Route("api/facilityLocations")]
        public HttpResponseMessage Post([FromBody]CreateWarehouseLocationParameter values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var input = values.Map().To<CreateLocationParameters>();
            _userIdentityProvider.SetUserIdentity(input);

            var result = _facilityService.CreateLocation(input);
            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }

        [System.Web.Http.Route("api/facilities/{id}/lock"),
        System.Web.Http.HttpPut,
        ClaimsAuthorize(ClaimActions.Full, ClaimTypes.WarehouseClaimTypes.WarehouseLocations)]
        public IEnumerable<FacilityLocationResponse> Freeze(string id, string streetName)
        {
            var locations = GetLocationsByFacilityAndStreet(id, streetName);
            var lockResult = _facilityService.LockLocations(locations);
            lockResult.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            return _facilityService.GetFacilities(id, true)
                .ResultingObject
                .SelectMany(w => w.Locations)
                .Project().To<FacilityLocationResponse>();
        }

        [System.Web.Http.Route("api/facilities/{id}/unlock"),
        System.Web.Http.HttpPut,
        ClaimsAuthorize(ClaimActions.Full, ClaimTypes.WarehouseClaimTypes.WarehouseLocations)]
        public IEnumerable<FacilityLocationResponse> Unlock(string id, string streetName)
        {
            var locations = GetLocationsByFacilityAndStreet(id, streetName);
            var lockResult = _facilityService.UnlockLocations(locations);
            lockResult.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            return _facilityService.GetFacilities(id, true)
                .ResultingObject
                .SelectMany(w => w.Locations)
                .Project().To<FacilityLocationResponse>();
        }

        private IEnumerable<string> GetLocationsByFacilityAndStreet(string id, string streetName)
        {
            var getWarehouse = _facilityService.GetFacilities(id, true);
            getWarehouse.EnsureSuccessWithHttpResponseException();

            return getWarehouse.ResultingObject.SelectMany(w => w.Locations)
                .Where(l => l.Status != LocationStatus.InActive)
                .Select(l => l)
                .FilterByStreet(streetName)
                .ToList()
                .Select(l => l.LocationKey);
        }
    }
}
