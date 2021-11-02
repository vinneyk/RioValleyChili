/** Components */
ko.components.register( 'sample-matching-editor', require('App/components/quality-control/sample-matching-editor/sample-matching-editor') );
ko.components.register( 'sample-matching-summary', require('App/components/quality-control/sample-matching-summary/sample-matching-summary') );
ko.components.register( 'product-selector', require('App/components/common/product-selector/product-selector'));
ko.components.register( 'product-maintenance-editor', require('App/components/quality-control/product-maintenance-editor/product-maintenance-editor'));

/** Required libraries */
var rvc = require('rvc');
var page = require('page');

/** Service libraries */
var qualityControlService = require('App/services/qualityControlService');
var directoryService = require('App/services/directoryService');
var productsService = require('App/services/productsService');
var salesService = require('App/services/salesService');
var lotService = require('App/services/lotService');

/** Required scripts */
require('bootstrap');
require('App/koExtensions');
require('App/helpers/koPunchesFilters');
ko.punches.enableAll();

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

/** Sample Matching base view model */
function SampleMatchingVM() {
  if ( !(this instanceof SampleMatchingVM) ) { return new SampleMatchingVM(); }

  var self = this;

  this.isInit = ko.observable( false );

  this.options = {
    sampleStatuses: [],
    companies: ko.observableArray( [] ),
    brokers: ko.observableArray( [] ),
    products: ko.observableArray( [] ),
    customerProducts: ko.observableArray( [] ),
    fobs: ko.observableArray( [] ),
    shipmentMethods: ko.observableArray( [] ),
    attrs: ko.observableArray( [] )
  };

  ko.utils.arrayForEach( Object.keys( rvc.lists.sampleStatusTypes ), function( sampleType ) {
    self.options.sampleStatuses.push( rvc.lists.sampleStatusTypes[ sampleType ] );
  });

  function init() {
    var getCompanies = directoryService.getCompanies().then(
    function( data ) {
      self.options.companies( data );
    });

    var getBrokers = directoryService.getBrokers().then(
    function( data ) {
      self.options.brokers( data );
    });

    var getAllProducts = getProducts().then(
    function( data ) {
      self.options.products( data );
    });

    function getProducts() {
      var productDfds = [];
      var products = {};

      function loadAndPush(type) {
        products[ type.key ] = ko.observable();

        var getProducts = productsService.getProductsByInventoryType( type.key )
        .done(function ( data ) {
          products[ type.key ]( data );
        });

        productDfds.push( getProducts );
      }

      rvc.helpers.forEachInventoryType(loadAndPush);

      var checkResults = $.when.apply($, productDfds).then(
      function() {
        return products;
      });

      return checkResults;
    }

    var getCustomerProducts = qualityControlService.getCustomerProductNames().then(
    function( data ) {
      var products = ko.utils.arrayMap( data || [], function( product ) {
        return { Name: product };
      });

      self.options.customerProducts( products );
    });

    var getFOBs = salesService.getWarehouses().then(
    function( data ) {
      var _mappedData = ko.utils.arrayMap( data, function( fob ) {
        return { Name: fob.FacilityName };
      });

      self.options.fobs( _mappedData );
    });

    var getShipmentMethods = salesService.getShipmentMethods().then(
    function( data ) {
      var _mappedData = ko.utils.arrayMap( data, function( shipmentMethod ) {
        return { Name: shipmentMethod };
      });

      self.options.shipmentMethods( _mappedData );
    });

    var checkOptions = $.when( getCompanies, getBrokers, getCustomerProducts, getFOBs, getShipmentMethods, getAllProducts ).then(
    function() {
      self.isInit( true );
      self.loadMoreSamples.execute();
      page();
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not start Sample Matching', {
        description: errorThrown
      });
    });

    return checkOptions;
  }

  // Data
  var _dateReceivedStart = ko.observableDate();
  var _dateReceivedEnd = ko.observableDate();
  var _dateCompletedStart = ko.observableDate();
  var _dateCompletedEnd = ko.observableDate();
  this.filters = {
    startDateReceivedFilter: _dateReceivedStart,
    endDateReceivedFilter: _dateReceivedEnd,
    startDateCompletedFilter: _dateCompletedStart,
    endDateCompletedFilter: _dateCompletedEnd,
    requestedCompanyKey: ko.observable(),
    brokerKey: ko.observable(),
    status: ko.observable()
  };

  // Create New Product modal
  this.newProductData = {
    input: ko.observable(),
    exports: ko.observable(),
  };
  var $newProductModal = $('#newProductModal');
  $newProductModal.on( 'hidden.bs.modal', function() {
    self.newProductData.input( null );
    self.newProductData.exports( null );
  });

  this.startNewProduct = ko.command({
    execute: function( product ) {
      var _productKey = ko.unwrap( product.ProductKey );
      _productKey = typeof _productKey === 'object' ?
        _productKey.ProductKey :
        _productKey;

      // Check for attribute names cache
      var getAttrNames;
      if ( self.options.attrs().length ) {
        // If cache exists, use data
        getAttrNames = $.Deferred().resolve( self.options.attrs() );
      } else {
        // Else, Fetch attr names
        getAttrNames = lotService.getAttributeNames().then(
        function( data ) {
          self.options.attrs( data[1] );
          return data[1];
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not get attribute names', {
            description: errorThrown
          });
        });
      }

      // Get Product default attrs
      var getAttrs = getAttrNames.then(function() {
        return productsService.getProductDetails( 1, _productKey ).then(
          function( data ) {
            return data;
          },
          function( jqXHR, textStatus, errorThrown ) {
            showUserMessage( 'Could not get attribute data', {
              description: errorThrown
            });
          });
      });

      // Set Lot Type, Chile Type, and Formulation
      var buildEditor = getAttrs.then(
      function( data ) {
        var _attrs = JSON.parse( ko.toJSON( self.options.attrs ) );
        var _editorData = {
          InventoryType: '1',
          ProductType: '1',
          attrs: { 1: _attrs },
          AttributeRanges: data.AttributeRanges,
          ProductIngredients: data.ProductIngredients,
          ChileTypeKey: data.ChileTypeKey
        };

        self.newProductData.input( _editorData );
        $newProductModal.modal('show');
      });

      return buildEditor;
    },
    canExecute: function() {
      return true;
    }
  });
  this.closeNewProductModal = ko.command({
    execute: function() {
      $newProductModal.modal( 'hide' );
    }
  });
  this.saveNewProduct = ko.asyncCommand({
    execute: function( complete ) {
      var editor = self.newProductData.exports();

      // If data is not valid, stop save and notify user
      if ( !editor.isValid() ) {
        showUserMessage( 'Could not save new product', {
          description: 'Please ensure all fields have been filled out correctly.'
        });

        return;
      }
      // Compile data
      var _productData = editor.toDto();

      // Save data to API
      var createProduct = productsService.createProduct( _productData ).then(
      function( data ) {
        // Close modal
        $newProductModal.modal( 'hide' );

        // Show user message with link to new product page
        showUserMessage( 'Product created successfully', {
          description: 'You have successfully created a new product. <a href="/QualityControl/ProudctMaintenance/1/' +
            data +
            '/" target="_blank">Click here to view it in the product maintenance UI</a>'
        });
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not save new product', {
          description: errorThrown
        });
      }).always( complete );

      return createProduct;
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  // Sample Matching Summaries component
  this.selected = ko.observable();
  this.summaryData = {
    input: ko.observableArray( [] ),
    selected: this.selected,
    exports: ko.observable()
  };
  this.selected.subscribe(function( sample ) {
    if ( sample ) {
      page( '/' + sample.SampleRequestKey );
    }
  });
  this.sampleMatchingPager = qualityControlService.buildSampleRequestsPager({
    onNewPageSet: function() {
      self.summaryData.input( [] );
    }
  });
  this.sampleMatchingPager.addParameters( this.filters );

  this.editorData = {
    input: ko.observable(),
    options: this.options,
    exports: ko.observable(),
    sampleRequestReportUrl: ko.pureComputed(function() {
      var input = this.editorData.input();

      if ( !input ) {
        return null;
      }

      return input.Links && input.Links["report-sample_request"].HRef;
    }, this),

    callbacks: {
      startNewProduct: this.startNewProduct,
    },
  };
  this.isEditorPicking = ko.pureComputed(function() {
    var editor = self.editorData.exports();

    return editor && editor.isPicking();
  });
  this.closePickerCommand = ko.command({
    execute: function() {
      self.editorData.exports().isPicking( false );
    }
  });
  this.closeEditorCommand = ko.asyncCommand({
    execute: function( complete ) {
      page( '/' );
      complete();
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  // Behaviors
  this.searchKey = ko.observable();
  this.searchForKey = ko.command({
    execute: function() {
      var _key = self.searchKey();

      page( '/' + _key );
    },
    canExecute: function() {
      return self.searchKey();
    }
  });

  this.loadMoreSamples = ko.asyncCommand({
    execute: function( complete ) {
      // Call pager for new data
      self.sampleMatchingPager.nextPage().then(
      function( data ) {
        var samples = self.summaryData.input;

        ko.utils.arrayPushAll( samples(), data );
        samples.valueHasMutated();
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.startNewSampleRequest = ko.command({
    execute: function() {
      page( '/new' );
    }
  });

  // Save sample matching data
  this.saveSample = ko.asyncCommand({
    execute: function( complete ) {
      var editor = self.editorData.exports();

      // If editor has valid data
      if ( editor && editor.isValid() ) {
        // Get editor data as a DTO
        var dto = editor.toDto();
        var sampleKey = dto.SampleRequestKey;
        var isNew = dto.SampleRequestKey == null;

        // If existing data, update sample request
        if ( !isNew ) {
          var updateSample = qualityControlService.updateSampleRequest( sampleKey, dto ).then(
            function( data ) {
            // Update summary table with new data
            self.summaryData.exports().updateSample( data );

            // Notify user of success
            showUserMessage( 'Save successful', {
              description: 'You have successfully updated the sample data for <b>' + sampleKey + '</b>.'
            });
          },
          function( jqXHR, textStatus, errorThrown ) {
            // Notify user of failure
            showUserMessage( 'Save failed', {
              description: errorThrown
            });
          }).always( complete );

          return updateSample;
        // Else, create new request
        } else {
          var createSample = qualityControlService.createSampleRequest( dto ).then(
            function( data ) {
            // Update summary table with new data
            self.summaryData.exports().updateSample( data );
            page( '/' + data.SampleRequestKey );

            // Notify user of success
            showUserMessage( 'Save successful', {
              description: 'You have successfully created a sample request.'
            });
          },
          function( jqXHR, textStatus, errorThrown ) {
            // Notify user of failure
            showUserMessage( 'Save failed', {
              description: errorThrown
            });
          }).always( complete );

          return createSample;
        }

      // Else, notify user of error and stop saving
      } else {
        showUserMessage( 'Could not save sample data', {
          description: 'Please ensure all fields are filled out correctly.'
        });
        complete();
      }
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });
  this.deleteSample = ko.asyncCommand({
    execute: function( complete ) {
      // Confirm before deleting
      showUserMessage( 'Delete sample request?', {
        description: 'Are you sure you want to delete this sample order? This action can\'t be undone.',
        type:'yesno',
        onYesClick: function() {
          var _key = self.editorData.input().SampleRequestKey;

          var deleteOrder = qualityControlService.deleteSampleRequest( _key ).then(
          function() {
            // Remove entry from summary table
            self.summaryData.exports().removeSample( _key );

            page.redirect( '/' );

            // Notify user of success
            showUserMessage('Sample request removed successfully');
          },
          function( jqXHR, textStatus, errorThrown ) {
            // Notify user of failure
            showUserMessage( 'Could not delete sample request', {
              description: errorThrown
            });
          }).always( complete );

          return deleteOrder;
        },
        onNoClick: complete
      });
    },
    canExecute: function( isExecuting ) {
      var editor = self.editorData.input();

      return !isExecuting && editor && editor.SampleRequestKey;
    }
  });

  this.isNew = ko.pureComputed(function() {
    var editor = self.editorData.input();

    return editor && editor.SampleRequestKey == null;
  });

  // Routing
  page.base('/QualityControl/SampleMatching');

  // Load details for sample
  function loadSampleDetails( ctx, next ) {
    var sampleKey = ctx.params.sampleKey;

    self.editorData.input( null );
    // If params have sample key
    if ( sampleKey === 'new' ) {
      self.editorData.input( {} );
    } else if ( sampleKey ) {
      // Load details for sample key
      var getDetails = qualityControlService.getSampleRequestDetails( sampleKey ).then(
      function( data ) {
        // Display editor on success
        self.editorData.input( data );
      },
      function() {
        // Redirect to summaries on failure
        page('/');
        showUserMessage( 'Could not find sample', {
          description: 'We could not find details matching the sample key <b>' + sampleKey + '</b>'
        });
      });

      return getDetails;

    // Else continue to next route
    } else {
      next();
    }
  }
  page( '/:sampleKey', loadSampleDetails );

  // Return to summary view and reset editor UI
  function displaySummaries() {
    self.editorData.input( null );
  }
  page( displaySummaries );

  init();

  // Exports
  return this;
}

var vm = new SampleMatchingVM();

ko.applyBindings( vm );

module.exports = vm;

