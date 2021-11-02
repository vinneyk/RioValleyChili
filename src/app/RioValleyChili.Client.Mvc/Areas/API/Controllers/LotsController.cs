using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Models.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.LotAttributes)]
    public class LotsController : ApiController
    {
        #region fields and constructors

        private readonly ILotService _lotService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public LotsController(ILotService lotService, IUserIdentityProvider userIdentityProvider)
        {
            if (lotService == null) { throw new ArgumentNullException("lotService"); }
            _lotService = lotService;

            if (userIdentityProvider == null) {  throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion
        
        // GET api/lots
        public LotSummariesResponse Get(LotTypeEnum? lotType = null, LotProductionStatus? productionStatus = null, LotQualityStatus? qualityStatus = null, string productKey = null, DateTime? productionStart = null, DateTime? productionEnd = null, string startingLotKey = null, string productSubType = null,
            int? pageSize = 50, int? skipCount = 0)
        {
            if(productionEnd.HasValue && productionStart.HasValue && productionStart.Value > productionEnd.Value)
            {
                var temp = productionStart.Value;
                productionStart = productionEnd;
                productionEnd = temp;
            }

            var filterParams = string.IsNullOrWhiteSpace(startingLotKey)
                ? new FilterLotParameters
                {
                    ProductType = ProductTypeEnum.Chile,
                    LotType = lotType,
                    ProductionStartRangeStart = productionStart.HasValue ? productionStart.Value.ToUniversalTime() : (DateTime?) null,
                    ProductionStartRangeEnd = productionEnd.HasValue ? productionEnd.Value.ToUniversalTime() : productionStart.HasValue ? DateTime.Now.ToUniversalTime() :  (DateTime?) null,
                    ProductionStatus = productionStatus,
                    QualityStatus = qualityStatus,
                    ProductKey = productKey
                }
                : new FilterLotParameters
                {
                    StartingLotKey = startingLotKey,
                };
            var result = _lotService.GetLotSummaries(filterParams);
            result.EnsureSuccessWithHttpResponseException();

            var lotQuery = result.ResultingObject.LotSummaries;
            if(!string.IsNullOrWhiteSpace(productSubType))
            {
                lotQuery = lotQuery.Where(l => l.LotProduct.ProductSubType == productSubType);
            }
            if(!productionStart.HasValue && string.IsNullOrWhiteSpace(startingLotKey))
            {
                lotQuery = lotQuery.OrderByDescending(l => l.LotDateCreated);
            }

            //var total = lotQuery.Count();

            var lotSummaries = lotQuery
                .PageResults(pageSize, skipCount)
                .Project().To<LotQualitySummaryResponse>();

            return new LotSummariesResponse
                {
                    AttributeNamesByProductType = result.ResultingObject.AttributeNamesByProductType,
                    LotSummaries = lotSummaries,
                    //Total = total
                };
        }

        // GET api/lots/5
        public async Task<LotDetailsResponse> Get(string lotKey)
        {
            var result = await Task.Run(() => _lotService.GetLotSummary(lotKey));
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject.Map().To<LotDetailsResponse>();
        }

        [System.Web.Http.Route("api/lots/{lotKey}/history"), System.Web.Http.HttpGet]
        public LotHistoryResponse GetLotHistory(string lotKey)
        {
            var result = _lotService.GetLotHistory(lotKey);
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject.Map().To<LotHistoryResponse>();
        }

        [System.Web.Http.Route("api/lots/{lotKey}/qualityStatus"),
        System.Web.Http.HttpPut,
        ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Full, ClaimTypes.QualityControlClaimTypes.LotStatus)]
        public HttpResponseMessage SetStatus(string lotKey, SetLotStatusDto values)
        {
            var input = new SetLotStatusParameter
                {
                    LotKey = lotKey,
                    QualityStatus = values.Status,
                };
            var result = _lotService.SetLotQualityStatus(_userIdentityProvider.SetUserIdentity(input));
            return result.ToMapped().Response<LotStatInfoResponse>(HttpVerbs.Put);
        }

        // PUT api/lots/5
        [ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Full, ClaimTypes.QualityControlClaimTypes.LotAttributes)]
        public async Task<HttpResponseMessage> Put(string lotKey, UpdateLotRequest values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = ModelState.GetErrorSummary()
                    };
            }
            
            values.LotKey = lotKey;
            
            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetLotAttributeParameters>());
            var result = await Task.Run(() => _lotService.SetLotAttributes(parameters));
            var lot = await Get(lotKey);

            lot.LotSummary.OldContextLotStat = result.ResultingObject.GetLotStatDescription();
                
            return new HttpResponseMessage(result.ToHttpStatusCode(HttpVerbs.Put))
                {
                    Content = new ObjectContent<LotQualitySummaryResponse>(lot.LotSummary, new JsonMediaTypeFormatter()),
                    ReasonPhrase = result.Message
                };
        }

        // PUT api/lots/addAttributes
        [System.Web.Http.Route("api/lots/addAttributes"),
        System.Web.Http.HttpPut,
        ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Full, ClaimTypes.QualityControlClaimTypes.LotAttributes)]
        public HttpResponseMessage AddAttributes([FromBody]AddLotAttributesRequest values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = ModelState.GetErrorSummary()
                    };
            }
            
            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<AddLotAttributesParameters>());
            var result = _lotService.AddLotAttributes(parameters);
            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        [System.Web.Http.Route("~/api/lots/countByQualityStatus"), System.Web.Http.AllowAnonymous]
        public IDictionary<LotQualityStatus, int> GetQualityControlSummary()
        {
            var lotSummaries = _lotService.GetLotSummaries(new FilterLotParameters
                {
                    ProductType = ProductTypeEnum.Chile,
                    ProductionStartRangeEnd = null,
                    ProductionStartRangeStart = null,
                    ProductionStatus = LotProductionStatus.Produced
                });

            var data = lotSummaries.ResultingObject.LotSummaries
                .Where(l => l.QualityStatus != LotQualityStatus.Released)
                .GroupBy(l => l.QualityStatus)
                .Select(l => new
                {
                    l.Key,
                    Count = l.Count(s => true)
                })
                .ToList()
                .ToDictionary(d => d.Key, d => d.Count);

            return data;
        }

        [System.Web.Http.HttpPut,
        System.Web.Http.Route("~/api/lots/{lotKey}/allowances/{type:values(customer|contract|order)}/{key}")]
        public HttpResponseMessage AddAllowance(string lotKey, string type, string key)
        {
            return ExecuteAllowance(lotKey, type, key, HttpVerbs.Put, _lotService.AddLotAllowance);
        }

        [System.Web.Http.HttpDelete,
        System.Web.Http.Route("~/api/lots/{lotKey}/allowances/{type:values(customer|contract|order)}/{key}")]
        public HttpResponseMessage DeleteAllowance(string lotKey, string type, string key)
        {
            return ExecuteAllowance(lotKey, type, key, HttpVerbs.Delete, _lotService.RemoveLotAllowance);
        }

        [System.Web.Http.HttpGet,
        System.Web.Http.Route("~/api/lots/{lotKey}/output-trace")]
        public IEnumerable<LotOutputTraceResponse> GetOutputTrace(string lotKey)
        {
            var result = _lotService.GetOutputTrace(lotKey);
            result.EnsureSuccessWithHttpResponseException();

            var response = result.ResultingObject.Project().To<LotOutputTraceResponse>();
            return response;
        }

        [System.Web.Http.HttpGet,
        System.Web.Http.Route("~/api/lots/{lotKey}/input-trace")]
        public IEnumerable<LotInputTraceResponse> GetInputTrace(string lotKey)
        {
            var result = _lotService.GetInputTrace(lotKey);
            result.EnsureSuccessWithHttpResponseException();

            var response = result.ResultingObject.Project().To<LotInputTraceResponse>();
            return response;
        }

        private static HttpResponseMessage ExecuteAllowance(string lotKey, string type, string key, HttpVerbs verb, Func<AllowanceParameters, IResult> action)
        {
            var parameters = new AllowanceParameters
                {
                    LotKey = lotKey
                };

            switch(type.ToUpper())
            {
                case "CUSTOMER": parameters.CustomerKey = key; break;
                case "CONTRACT": parameters.ContractKey = key; break;
                case "ORDER": parameters.CustomerOrderKey = key; break;
                default: return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(string.Format("Invalid allowance type '{0}'.", type))
                    };
            }

            return action(parameters).ToHttpResponseMessage(verb);
        }
    }
}
