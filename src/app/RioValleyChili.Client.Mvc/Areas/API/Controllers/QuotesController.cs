using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class QuotesController : ApiController
    {
        public QuotesController(ISalesService salesService, IUserIdentityProvider userIdentityProvider)
        {
            if(salesService == null) { throw new ArgumentNullException("salesService"); }
            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider"); }
            
            _salesService = salesService;
            _userIdentityProvider = userIdentityProvider;
        }

        public IEnumerable<SalesQuoteSummaryResponse> Get(string customerKey = null, string brokerKey = null, DateTime? quoteDateStart = null, DateTime? quoteDateEnd = null, int skipCount = 0, int pageSize = 20)
        {
            var result = _salesService.GetSalesQuotes(new FilterSalesQuotesParameters
                {
                    CustomerKey = customerKey,
                    BrokerKey = brokerKey
                });
            result.EnsureSuccessWithHttpResponseException();

            quoteDateStart = quoteDateStart != null ? quoteDateStart.Value.Date : (DateTime?) null;
            quoteDateEnd = quoteDateEnd != null ? quoteDateEnd.Value.Date.AddDays(1) : (DateTime?) null;

            var mapped = result.ResultingObject
                .Where(q => (quoteDateStart == null || q.QuoteDate >= quoteDateStart) && (quoteDateEnd == null || q.QuoteDate < quoteDateEnd))
                .OrderBy(q => q.QuoteDate)
                .PageResults(pageSize, skipCount)
                .Project().To<SalesQuoteSummaryResponse>();
            return mapped;
        }

        [System.Web.Http.HttpGet, System.Web.Http.Route("api/quotes/{quoteNumber}")]
        public SalesQuoteDetailResponse Get(int quoteNumber)
        {
            var result = _salesService.GetSalesQuote(quoteNumber);
            result.EnsureSuccessWithHttpResponseException();
            return Initialize(result.ResultingObject.Map().To<SalesQuoteDetailResponse>());
        }

        public SalesQuoteDetailResponse Post([FromBody]CreateSalesQuoteRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SalesQuoteParameters>());
            var create = _salesService.SetSalesQuote(parameters);
            create.EnsureSuccessWithHttpResponseException(HttpVerbs.Post);

            var detail = _salesService.GetSalesQuote(create.ResultingObject);
            detail.EnsureSuccessWithHttpResponseException();
            return Initialize(detail.ResultingObject.Map().To<SalesQuoteDetailResponse>());
        }

        [System.Web.Http.HttpPut, System.Web.Http.Route("api/quotes/{quoteNumber}")]
        public SalesQuoteDetailResponse Put(int quoteNumber, [FromBody]UpdateSalesQuoteRequest values)
        {
            ModelState.EnsureValidModelStateWithHttpResponseException();

            var parameters = _userIdentityProvider.SetUserIdentity(values.Map().To<SalesQuoteParameters>());
            parameters.SalesQuoteNumber = quoteNumber;
            var update = _salesService.SetSalesQuote(parameters, true);
            update.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);

            var detail = _salesService.GetSalesQuote(update.ResultingObject);
            detail.EnsureSuccessWithHttpResponseException();
            return Initialize(detail.ResultingObject.Map().To<SalesQuoteDetailResponse>());
        }

        private SalesQuoteDetailResponse Initialize(SalesQuoteDetailResponse details)
        {
            details.Links.Add(this.BuildReportLink(MVC.Reporting.SalesReporting.SalesQuoteReport(details.QuoteNumber ?? 0), "report-sales-quote"));
            return details;
        }

        private readonly ISalesService _salesService;
        private readonly IUserIdentityProvider _userIdentityProvider;
    }
}