function InventoryAdjustmentsSummaryVM( params ) {
  if ( !(this instanceof InventoryAdjustmentsSummaryVM) ) { return new InventoryAdjustmentsSummaryVM( params ); }

  var self = this;

  // Data
  var selected = params.selected || ko.observable( null );

  this.adjustmentSummaries = params.input;

  // Behaviors
  this.selectSummary = function( data, $element ) {
    $tr = $( $element.target ).closest('tr')[0];

    if ( $tr ) {
      selected( ko.contextFor( $tr ).$data );
    }
  };

  // Exports
  if ( params && params.exports ) {
    params.exports({
    });
  }

  return this;
}

module.exports = {
  viewModel: InventoryAdjustmentsSummaryVM,
  template: require('./inventory-adjustments-summary.html')
};
