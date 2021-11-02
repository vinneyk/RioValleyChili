function SummaryItem( summaryData ) {
  this.SalesQuoteKey = summaryData.SalesQuoteKey;
  this.QuoteNumber = summaryData.QuoteNumber;
  this.QuoteDate = summaryData.QuoteDate;
  this.ScheduledShipDate = summaryData.Shipment.ShippingInstructions.ScheduledShipDateTime;
  this.CustomerName = summaryData.Customer && summaryData.Customer.Name;
  this.BrokerName = summaryData.Broker && summaryData.Broker.Name;
  this.SourceFacilityName = summaryData.SourceFacility && summaryData.SourceFacility.FacilityName;
}

function QuotesSummaryVM( params ) {
  if ( !(this instanceof QuotesSummaryVM) ) { return new QuotesSummaryVM( params ); }

  var self = this;

  this.quotes = params.input;
  this.selected = params.selected;

  this.selectQuote = function( quote ) {
    self.selected( quote );
  };

  function addSummary( summaryData ) {
    self.quotes.unshift( new SummaryItem( summaryData ) );
  }

  function updateSummary( summaryData ) {
    var _key = summaryData.SalesQuoteKey;
    var _quotes = self.quotes();

    var _quote = ko.utils.arrayFirst( _quotes, function( quote ) {
      return quote.SalesQuoteKey === _key;
    } );

    if ( _quote ) {
      var i = _quotes.indexOf( _quote );

      self.quotes.splice( i, 1, new SummaryItem( summaryData ) );

      return;
    }

    addSummary( summaryData );
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      addSummary: addSummary,
      updateSummary: updateSummary
    });
  }

  return this;
}

module.exports = {
  viewModel: QuotesSummaryVM,
  template: require('./quotes-summary.html')
};
