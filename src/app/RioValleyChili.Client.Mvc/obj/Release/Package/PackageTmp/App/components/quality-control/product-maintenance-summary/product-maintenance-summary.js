// TODO NJH: Add sticky headers and column sorting

/** Product maintenance summary table that displays input data
  *
  * @param {Type} input - Observable, Products to display in the summary table
  * @param {Type} selected - Observable, used to monitor which item has been selected
  */
function ProductMaintenanceSummaryVM( params ) {
  if ( !(this instanceof ProductMaintenanceSummaryVM) ) { return new ProductMaintenanceSummaryVM( params ); }

  var self = this;

  // Data
  this.products = params.input;
  this.selected = params.selected;

  this.isTableVisible = ko.pureComputed(function() {
    var products = self.products();

    return products && products.length > 0;
  });

  // Behaviors
  this.selectItem = function( data, element ) {
    // Verify clicked element is or is a child of a tr element
    var $tr = $( element.target ).closest('tr');

    // If element has a tr parent
    if ( $tr.length ) {
      // Get context of element
      var context = ko.contextFor( $tr[0] ).$data;

      // Change selected item to the selected element
      self.selected( context.ProductKey );
    }
  };

   function updateItem( data ) {
    var item = ko.utils.arrayFirst( ko.unwrap( self.products ), function( product ) {
      return product.ProductKey === data.ProductKey;
    });

    // Products data is computed, not observableArray
    var prodData = self.products();

    if ( item ) {
      var index = self.products().indexOf( item );

      // Update item info if in table
      if ( index > -1 ) {
        prodData.splice( index, 1, data );
        self.products( prodData );

      // Else append to table
      } else {
        self.products( [ data ].concat( prodData ) );
      }
    } else {
      // Append item to table
      self.products( [ data ].concat( prodData ) );
    }
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      updateItem: updateItem
    });
  }

  return this;
}

module.exports = {
  viewModel: ProductMaintenanceSummaryVM,
  template: require('./product-maintenance-summary.html')
};


