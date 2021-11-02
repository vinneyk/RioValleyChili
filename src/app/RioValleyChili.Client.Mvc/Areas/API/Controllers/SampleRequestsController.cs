using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.SampleRequests;
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
    public class SampleRequestsController : ApiController
    {
        private readonly ISampleOrderService _sampleOrderService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public SampleRequestsController(ISampleOrderService customerService, IUserIdentityProvider userIdentityProvider)
        {
            if(customerService == null) { throw new ArgumentNullException("customerService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            
            _sampleOrderService = customerService;
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<SampleRequestSummaryResponse> Get(DateTime? startDateReceivedFilter = null, DateTime? endDateReceivedFilter = null, DateTime? startDateCompletedFilter = null, DateTime? endDateCompletedFilter = null, string requestedCompanyKey = null, string brokerKey = null, SampleOrderStatus? status = null,
            int skipCount = 0, int pageSize = 20)
        {
            var result = _sampleOrderService.GetSampleOrders(new FilterSampleOrdersParameters
                {
                    DateReceivedStart = startDateCompletedFilter,
                    DateReceivedEnd = startDateReceivedFilter,
                    DateCompletedStart = startDateCompletedFilter,
                    DateCompletedEnd = endDateCompletedFilter,
                    RequestedCompanyKey = requestedCompanyKey,
                    BrokerKey = brokerKey,
                    Status = status
                });
            result.EnsureSuccessWithHttpResponseException();

            var mapped = result.ResultingObject
                .OrderByDescending(o => o.DateReceived)
                .PageResults(pageSize, skipCount)
                .Project().To<SampleRequestSummaryResponse>();
            return mapped;
        }

        public SampleRequestDetailResponse Get(string id)
        {
            var result = _sampleOrderService.GetSampleOrder(id);
            result.EnsureSuccessWithHttpResponseException();

            var response = result.ResultingObject.Map().To<SampleRequestDetailResponse>();
            var summaryReportLink = this.BuildReportLink(MVC.Reporting.SampleOrderReporting.SampleMatchingSummary(response.SampleRequestKey), "report-sample_match_summary");
            var requestReportLink = this.BuildReportLink(MVC.Reporting.SampleOrderReporting.SampleRequest(response.SampleRequestKey), "report-sample_request");
            response.Links = new ResourceLinkCollection
                {
                    summaryReportLink,
                    requestReportLink
                };
            foreach(var item in response.Items)
            {
                item.Links = new ResourceLinkCollection
                    {
                        this.BuildReportLink(MVC.Reporting.SampleOrderReporting.SampleMatchingSummary(response.SampleRequestKey, item.ItemKey), "report-sample_match_summary")
                    };
            }
            return response;
        }

        public HttpResponseMessage Post([FromBody]CreateSampleOrderRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetSampleOrderParameters>());
            var result = _sampleOrderService.SetSampleOrder(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);

            return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = Get(result.ResultingObject).ToJSONContent()
                };
        }

        public HttpResponseMessage Put(string id, [FromBody]SetSampleOrderRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetSampleOrderParameters>());
            parameters.SampleOrderKey = id;
            var result = _sampleOrderService.SetSampleOrder(parameters);
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = Get(result.ResultingObject).ToJSONContent()
                };
        }

        public HttpResponseMessage Delete(string id)
        {
            return _sampleOrderService.DeleteSampleOrder(id).ToHttpResponseMessage(HttpVerbs.Delete);
        }

        [System.Web.Http.Route("api/samplerequests/{id}/items/{itemKey}/customerspecs"), System.Web.Http.HttpPut]
        public HttpResponseMessage Put(string id, string itemKey, [FromBody]SetSampleSpecsRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = values.Map().To<SetSampleSpecsParameters>();
            parameters.SampleOrderItemKey = itemKey;
            var result = _sampleOrderService.SetSampleSpecs(parameters);

            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        [System.Web.Http.Route("api/samplerequests/{id}/items/{itemKey}/labresults"), System.Web.Http.HttpPut]
        public HttpResponseMessage Put(string id, string itemKey, [FromBody]SetSampleMatchRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = values.Map().To<SetSampleMatchParameters>();
            parameters.SampleOrderItemKey = itemKey;
            var result = _sampleOrderService.SetSampleMatch(parameters);

            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        [System.Web.Http.Route("api/samplerequests/{id}/journals"), System.Web.Http.HttpPost]
        public HttpResponseMessage Post(string id, [FromBody]SetJournalEntryRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetSampleOrderJournalEntryParameters>());
            parameters.SampleOrderKey = id;
            var result = _sampleOrderService.SetJournalEntry(parameters);

            var mapped = result.ResultingObject.Map().To<SampleRequestJournalEntryResponse>();
            return result.ConvertTo(mapped).ToHttpResponseMessage(HttpVerbs.Post);
        }

        [System.Web.Http.Route("api/samplerequests/{id}/journals/{entryKey}"), System.Web.Http.HttpPut]
        public HttpResponseMessage Put(string id, string entryKey, [FromBody]SetJournalEntryRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SetSampleOrderJournalEntryParameters>());
            parameters.SampleOrderKey = id;
            parameters.JournalEntryKey = entryKey;
            var result = _sampleOrderService.SetJournalEntry(parameters);

            var mapped = result.ResultingObject.Map().To<SampleRequestJournalEntryResponse>();
            return result.ConvertTo(mapped).ToHttpResponseMessage(HttpVerbs.Put);
        }

        [System.Web.Http.Route("api/samplerequests/{id}/journals/{entryKey}"), System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete(string id, string entryKey)
        {
            return _sampleOrderService.DeleteJournalEntry(entryKey).ToHttpResponseMessage(HttpVerbs.Post);
        }

        [System.Web.Http.Route("api/samplerequests/customerproductnames"), System.Web.Http.HttpGet]
        public HttpResponseMessage GetCustomerProductNames()
        {
            return _sampleOrderService.GetCustomerProducNames().ToHttpResponseMessage();
        }
    }
}