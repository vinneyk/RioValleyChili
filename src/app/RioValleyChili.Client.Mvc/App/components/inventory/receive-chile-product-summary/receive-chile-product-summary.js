var inventoryService = require('App/services/inventoryService.js');

function ReceiveChileProductSummaryVM ( params ) {
  if ( !(this instanceof ReceiveChileProductSummaryVM) ) { return new ReceiveChileProductSummaryVM( params ); }

  var self = this;

  // Data
  this.summaryItems = params.input;

  // Behaviors
  function addEntry( newEntry ) {
    self.summaryItems.unshift( newEntry );
  }

  function updateEntry( newEntry ) {
    var items = self.summaryItems();
    var itemIndex;

    var i;
    var len = items.length;
    for ( i = 0; i < len; i++ ) {
      if ( items[i].LotKey === newEntry.LotKey ) {
        itemIndex = i;
        break;
      }
    }
    // If matching entry, replace data
    if ( itemIndex != null ) {
      self.summaryItems.splice( itemIndex, 1, newEntry );
    // Else, append to summary table
    } else {
      addEntry( newEntry );
    }
  }

  var selected = params.selected;
  this.selectItem = function( vm, element ) {
    var $tr = $( element.target ).closest('tr')[0];

    if ( $tr ) {
      var context = ko.contextFor( $tr ).$data;
      selected( context );
    }
  };

  // Exports
  if ( params.exports ) {
    params.exports({
      updateEntry: updateEntry,
      addEntry: addEntry
    });
  }

  return this;
}

module.exports = {
  viewModel: ReceiveChileProductSummaryVM,
  template: require('./receive-chile-product-summary.html')
};
