/** Customer Product Spec Editor view model
  *
  * @param {Object} input - Observable, products to display in summary view
  * @param {Object} selected - Observable, container for selected summary item
  * @param {Object} exports - Observable, container for exported methods and properties
  */
function CustomerProductSpecSummaryVM( params ) {
  if ( !(this instanceof CustomerProductSpecSummaryVM) ) { return new CustomerProductSpecSummaryVM( params ); }

  var self = this;

  // Data
  // Customer products to display in table
  this.products = params.input;

  // Selected product
  this.selected = params.selected;

  // Behaviors
  // Select product for editing
  this.selectProduct = function( data, element ) {
    // Verify element is a table item
    var $tr = $( element.target ).closest('tr')[0];

    if ( $tr ) {
      // Get context of element and set as the currently selected item
      var productData = ko.contextFor( $tr ).$data;
      self.selected( productData );
    }
  };

  // Update product data in table
  function updateProduct( newProductData ) {
    // Search for product in table
    var _products = self.products();
    var matchedProduct = ko.utils.arrayFirst( _products, function( product ) {
      return product.ChileProduct.ProductKey === newProductData.ChileProduct.ProductKey;
    });

    // If product exists in table, update it's information
    if ( matchedProduct ) {
      var productIndex = _products.indexOf( matchedProduct );

      _products.splice( productIndex, 1, newProductData );
      self.products( _products );

    // Else, append new data to table
    } else {
      self.products( [ newProductData ].concat( _products ) );
    }
  }

  function removeProduct( productKey ) {
    // Search for product in table
    var _products = self.products();
    var matchedProduct = ko.utils.arrayFirst( _products, function( product ) {
      return product.ChileProduct.ProductKey === productKey;
    });

    // If product exists in table, remove it
    if ( matchedProduct ) {
      var productIndex = _products.indexOf( matchedProduct );

      _products.splice( productIndex, 1 );
      self.products( _products );
    }
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      updateProduct: updateProduct,
      removeProduct: removeProduct
    });
  }

  return this;
}

module.exports = {
  viewModel: CustomerProductSpecSummaryVM,
  template: require('./customer-product-spec-summary.html')
};
