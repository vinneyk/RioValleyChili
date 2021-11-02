function ProductionSchedulesSummaryVM( params ) {
  if ( !(this instanceof ProductionSchedulesSummaryVM) ) { return new ProductionSchedulesSummaryVM( params ); }

  var self = this;

  this.summaries = params.input;
  this.selected = params.selected;

  this.selectSummary = function( data, element ) {
    var $tr = $( element.target ).closest('tr')[0];

    if ( $tr ) {
      var _summaryData = ko.contextFor( $tr ).$data;
      self.selected( _summaryData );
    }
  };

  function addSummary( summaryData ) {
    var _summaries = self.summaries();

    self.summaries.unshift( summaryData );

    self.summaries.sort(function compare( a, b ) {
      var _dateA = new Date( a.ProductionDate );
      var _dateB = new Date( b.ProductionDate );

      if ( _dateA < _dateB ) {
        return 1;
      }

      if ( _dateA > _dateB ) {
        return -1;
      }

      return 0;
    });
  }

  function removeSummary( summaryKey ) {
    var _summaries = self.summaries();
    var _summary = ko.utils.arrayFirst( _summaries, function( summary ) {
      return summary.ProductionScheduleKey === summaryKey;
    });
    var _summaryIndex = _summaries.indexOf( _summary );

    if ( _summary ) {
      self.summaries.splice( _summaryIndex, 1 );
    }
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      addSummary: addSummary,
      removeSummary: removeSummary
    });
  }

  return this;
}

module.exports = {
  viewModel: ProductionSchedulesSummaryVM,
  template: require('./production-schedules-summary.html')
};
