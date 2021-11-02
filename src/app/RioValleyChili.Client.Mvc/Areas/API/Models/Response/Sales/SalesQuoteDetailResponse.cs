using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesQuoteDetailResponse : ILinkedResource<SalesQuoteDetailResponse>
    {
        public string SalesQuoteKey { get; set; }
        public int? QuoteNumber { get; set; }
        public DateTime QuoteDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public string CalledBy { get; set; }
        public string TakenBy { get; set; }
        public string PaymentTerms { get; set; }

        public ShipmentDetails Shipment { get; set; }
        public FacilityResponse SourceFacility { get; set; }
        public CompanyResponse Customer { get; set; }
        public CompanyResponse Broker { get; set; }
        public IEnumerable<SalesQuoteItemResponse> Items { get; set; }

        public ResourceLinkCollection Links { get; set; }
        public string HRef { get { return Links.SelfHRef; } }
        public SalesQuoteDetailResponse Data { get { return this; } }
    }
}