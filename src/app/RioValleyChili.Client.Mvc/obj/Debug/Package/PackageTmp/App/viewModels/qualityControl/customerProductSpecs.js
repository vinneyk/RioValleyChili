/** Required libraries */
var lotService = require('App/services/lotService');
var productsService = require('App/services/productsService');
var directoryService = require('App/services/directoryService');
var page = require('page');

/** Extensions */
require('App/koExtensions');

/** Validation init */
ko.validation.init({
  insertMessages: false,
  decorateInputElement: true,
  errorElementClass: 'has-error',
  errorMessageClass: 'help-block',
  grouping: {
    deep: true,
    live: true,
    observable: true
  }
});

/** Included components */
ko.components.register( 'customer-product-spec-editor', require('App/components/quality-control/customer-product-spec-editor/customer-product-spec-editor'));
ko.components.register( 'customer-product-spec-summary', require('App/components/quality-control/customer-product-spec-summary/customer-product-spec-summary'));
ko.components.register( 'product-selector', require('App/components/common/product-selector/product-selector'));

/** Customer Product Spec view model */
function CustomerProductSpecsVM() {
  if ( !(this instanceof CustomerProductSpecsVM) ) { return new CustomerProductSpecsVM( params ); }

  var self = this;
  this.isInit = ko.observable( false );

  // Data
  // Options data
  this.options = {
    attributes: ko.observableArray( [] ),
    customers: ko.observableArray( [] ),
    products: ko.observableArray( [] )
  };

  // Summary Data
  this.selectedCustomer = ko.observable();
  var _getProductsForCustomer = this.selectedCustomer.subscribe(function( customerData ) {
    if ( customerData != null && typeof customerData === "object" ) {
      getCustomerProductOverrides( customerData.CompanyKey ).then(
      function( data, textStatus, jqXHR ) {
        self.summaryData.input( data );
      });
    }
  });

  var _productsCache = {};
  function getCustomerProductOverrides( customerKey ) {
    if ( _productsCache[ customerKey ] ) {
      return $.Deferred().resolve( _productsCache[ customerKey ] );
    }

    var getProducts = productsService.getCustomerProducts( customerKey ).then(
    function( data, textStatus, jqXHR ) {
      _productsCache[ customerKey ] = data;

      return data;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get customer products', {
        description: errorThrown
      });
    });

    return getProducts;
  }

  this.selectedProduct = ko.observable();
  var _getProductDetails = this.selectedProduct.subscribe(function( productData ) {
    page( '/' + self.selectedCustomer().CompanyKey + '/' + productData.ChileProduct.ProductKey );
  });
  this.summaryData = {
    input: ko.observableArray( [] ),
    selected: this.selectedProduct,
    exports: ko.observable()
  };

  // Editor Data
  this.editorData = {
    input: ko.observable(),
    options: this.options,
    exports: ko.observable()
  };

  this.isNew = ko.pureComputed(function() {
    var editorInput = self.editorData.input();

    return editorInput && !editorInput.ChileProductKey;
  });

  // Behaviors
  this.startNewOverrideCommand = ko.command({
    execute: function() {
      page( '/' + self.selectedCustomer().CompanyKey + '/new' );
    },
    canExecute: function() {
      return self.selectedCustomer();
    }
  });

  // Save editor data
  this.saveOverrideCommand = ko.asyncCommand({
    execute: function( complete ) {
    var editor = self.editorData.exports();

    // If data is invalid
    if ( !editor.isValid() ) {
      // Stop saving and notify user
      showUserMessage( 'Could not save product override', {
        description: 'Please ensure all required fields are filled out correctly.'
      });

      complete();
      return;
    }

    // Compile DTO from editor and begin save
    var _dto = editor.toDto();
    var _customerKey = _dto.CustomerKey;
    var _productKey = _dto.ChileProduct.ProductKey;

    // If new
    var createOverride = productsService.createCustomerProductOverride( _customerKey, _productKey, _dto.AttributeRanges );

    // After calling the save API
    var finishSave = createOverride.then(
    function( data, textStatus, jqXHR ) {
      // Get updated data
      productsService.getCustomerProductDetails( _customerKey, _productKey ).then(
      function( data, textStatus, jqXHR ) {
        // Workaround due to API only providing AttributeRanges
        var _updateData = {
          AttributeRanges: data,
          ChileProduct: _dto.ChileProduct
        };

        // Update summary table
        self.summaryData.exports().updateProduct( _updateData );

        // Notify User of success and close UI
        showUserMessage( 'Save successful', {
          description: 'You have successfully created a product override for <b>' + _dto.ChileProduct.ProductCodeAndName + '</b>.'
        });
        page( '/' );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not get updated product details', {
          description: errorThrown
        });
      });
    },
    function( jqXHR, textStatus, errorThrown ) {
      // Notify use of failed save
      showUserMessage( 'Could not save product override');
    }).always( complete );

    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  // Delete current override
  this.deleteOverrideCommand = ko.asyncCommand({
    execute: function( complete ) {
      var editorInput = self.editorData.input();
      var _customerKey = editorInput.CustomerKey;
      var _productKey = editorInput.ChileProductKey;

      showUserMessage( 'Delete override?', {
        description: 'Are you sure you want to remove this product override? This action cannot be undone.',
        type: 'yesno',
        onYesClick: function() {
          productsService.deleteCustomerProductOverride( _customerKey, _productKey ).then(
          function( data, textStatus, jqXHR ) {
            // Remove from summary table
            self.summaryData.exports().removeProduct( _productKey );

            // Notify user of success and close UI
            showUserMessage( 'Product override removed successfully', {
              description: 'You have removed the product override.'
            });
            page( '/' );
          },
          function( jqXHR, textStatus, errorThrown ) {
            // Notify user of failure
            showUserMessage( 'Failed to delete product override', {
              description: errorThrown
            });
          }).always( complete );
        },
        onNoClick: function() {
          complete();
        }
      });
    },
    canExecute: function( isExecuting ) {
      var _selectedProduct = self.selectedProduct();

      return !isExecuting && _selectedProduct && _selectedProduct.ChileProduct != null;
    }
  });

  // Close editor
  function closeEditor() {
    // If editor is dirty, confirm if user wants to save before closing
    if ( false ) {
      // If yes, attempt to save and navigate if successful, else stop closing
      // If no, close UI and discard changes
      // If cancel, do nothing

    // Else, close UI
    } else {
      self.editorData.input( null );
    }
  }

  // Close editor view via command
  this.closeEditor = ko.command({
    execute: function() {
      // Navigate to summary view to trigger route
      page( '/' );
    }
  });

  // Page.js Routing
  page.base('/QualityControl/CustomerSpecs');

  // Check if editor is open and if it's dirty
  function checkIfDirty( ctx, next ) {
    // If dirty, confirm with user and offer to save
    if ( false ) {

    // Else, continue with navigation
    } else {
      next();
    }
  }
  page( checkIfDirty );

  // Check if user provided key
  function getProductDetails( ctx, next ) {
    var _customerKey = ctx.params.customerKey;
    var _productKey = ctx.params.productKey;

    // If user provided 'new' key, display "Create" view
    if ( _productKey === 'new' ) {
      self.editorData.input({
        CustomerKey: _customerKey,
        ChileProductKey: null
      });
    // Else, if user provided an existing key, display "Edit" view
    } else if ( _productKey ) {
      // Get data for product
      var getProduct = productsService.getCustomerProductDetails( _customerKey, _productKey ).then(
      function( data, textStatus, jqXHR ) {
        // Set editor input to fetched data
        data.CustomerKey = _customerKey;
        self.editorData.input({
          AttributeRanges: data,
          ChileProductKey: _productKey,
          CustomerKey: _customerKey
        });
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not get product override details', {
          description: errorThrown
        });
        page.redirect( '/' );
      });
    // Else, continue to next route
    } else {
      next();
    }
  }
  page( '/:customerKey/:productKey?', getProductDetails );

  function displaySummaries( ctx, next ) {
    // If user did not provide key, close editor UI
    closeEditor();
  }
  page( displaySummaries );

  // Fetch options and ensure all data is ready before initializing UI
  (function init() {
    var getCustomers = directoryService.getCustomers().then(
      function( data, textStatus, jqXHR ) {
      self.options.customers( data );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get customers', {
        description: errorThrown
      });
    });

    var getProducts = productsService.getChileProducts().then(
    function( data, textStatus, jqXHR ) {
      self.options.products( data );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get products', {
        description: errorThrown
      });
    });

    var getAttributes = lotService.getAttributeNames().then(
    function( data, textStatus, jqXHR ) {
      self.options.attributes( data[1] );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get products', {
        description: errorThrown
      });
    });

    var checkOptions = $.when( getCustomers, getProducts ).then(
      function( data, textStatus, jqXHR ) {
      self.isInit( true );
    });

    return checkOptions;
  })();

  // Exports
  return this;
}

var vm = new CustomerProductSpecsVM();

ko.applyBindings( vm );

module.exports = vm;
