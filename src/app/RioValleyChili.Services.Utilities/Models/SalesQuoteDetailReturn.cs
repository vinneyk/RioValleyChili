using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesQuoteDetailReturn : ISalesQuoteDetailReturn
    {
        public string SalesQuoteKey { get { return SalesQuoteKeyReturn.SalesQuoteKey; } }
        public int? QuoteNumber { get; internal set; }
        public DateTime QuoteDate { get; internal set; }
        public DateTime? DateReceived { get; internal set; }
        public string CalledBy { get; internal set; }
        public string TakenBy { get; internal set; }
        public string PaymentTerms { get; set; }
        public ShippingLabel ShipFromReplace { get; internal set; }
        public IShipmentDetailReturn Shipment { get; internal set; }
        public IFacilitySummaryReturn SourceFacility { get; internal set; }
        public ICompanySummaryReturn Customer { get; internal set; }
        public ICompanySummaryReturn Broker { get; internal set; }
        public IEnumerable<ISalesQuoteItemReturn> Items { get; internal set; }

        internal SalesQuoteKeyReturn SalesQuoteKeyReturn { get; set; }

        
    }
}