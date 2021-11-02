using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class MillWetdownController : ApiController
    {
        #region fields and constructors

        readonly IMillAndWetDownService _millAndWetDownService;
        readonly IUserIdentityProvider _userIdentityProvider;

        public MillWetdownController(IMillAndWetDownService millAndWetDownService, IUserIdentityProvider userIdentityProvider)
        {
            if (millAndWetDownService == null) { throw new ArgumentNullException("millAndWetDownService"); }
            _millAndWetDownService = millAndWetDownService;
            
            if (userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion

        // GET api/millandwetdown
        public IEnumerable<MillAndWetdownSummary> Get(DateTime? startDate = null, DateTime? endDate = null, string lineKey = null, int? skipCount = null, int? pageSize = null)
        {
            var result = _millAndWetDownService.GetMillAndWetdownSummaries();
            result.EnsureSuccessWithHttpResponseException();
            var resultData = result.ResultingObject;

            if(startDate.HasValue)
            {
                if(startDate > endDate)
                {
                    var temp = startDate.Value;
                    startDate = endDate.Value;
                    endDate = temp;
                }

                var beginDateRange = startDate.Value.Date;
                var endDateRange = endDate.HasValue ? endDate.Value.Date.AddDays(1) : (DateTime?)null;
                var includeAllLaterDates = !endDate.HasValue;

                resultData = resultData.Where(d =>
                    d.ProductionBegin >= beginDateRange
                    && (includeAllLaterDates || d.ProductionBegin < endDateRange));

                resultData = resultData.OrderBy(r => r.ProductionBegin);
            }
            else
            {
                resultData = resultData.OrderByDescending(r => r.ProductionBegin);
            }

            return resultData.PageResults(pageSize, skipCount)
                .Project().To<MillAndWetdownSummary>();
        }

        // GET api/millandwetdown/5
        public MillAndWetdownDetail Get(string id) 
        {
            var result = _millAndWetDownService.GetMillAndWetdownDetail(id);
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject.Map().To<MillAndWetdownDetail>();
        }

        // POST api/millandwetdown
        public void Post([FromBody]CreateMillAndWetdownRequest value)
        {
            if(!ModelState.IsValid || value == null) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var dto = value.Map().To<CreateMillAndWetdownParameters>();
            var result = _millAndWetDownService.CreateMillAndWetdown(_userIdentityProvider.SetUserIdentity(dto));
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);
        }

        // PUT api/millandwetdown
        public void Put([FromBody]UpdateMillAndWetdownRequest value)
        {
            if(!ModelState.IsValid || value == null) { throw new HttpResponseException(HttpStatusCode.BadRequest); }

            var dto = value.Map().To<UpdateMillAndWetdownParameters>();
            var result = _millAndWetDownService.UpdateMillAndWetdown(_userIdentityProvider.SetUserIdentity(dto));
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }

        public void Delete(string id)
        {
            var result = _millAndWetDownService.DeleteMillAndWetdown(id);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Delete);
        }
    }
}
