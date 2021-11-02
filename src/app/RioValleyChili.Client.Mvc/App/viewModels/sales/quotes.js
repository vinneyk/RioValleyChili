/** Required libraries */
var rvc = require('rvc');
var page = require('page');
var salesService = require('App/services/salesService');
var directoryService = require('App/services/directoryService');
var warehouseService = require('App/services/warehouseService');
var productsService = require('App/services/productsService');

/** KO Components */
ko.components.register( 'quotes-summary', require('App/components/sales/quotes-summary/quotes-summary') );
ko.components.register( 'quotes-editor', require('App/components/sales/quotes-editor/quotes-editor') );
ko.components.register( 'loading-screen', require('App/components/common/loading-screen/loading-screen') );

require('App/koBindings');
require('App/helpers/koDeepValidationHelpers');

require('App/helpers/koPunchesFilters');
ko.punches.enableAll();

/** Quotes view model */
function QuotesVM() {
  if ( !(this instanceof QuotesVM) ) { return new QuotesVM(); }

  var self = this;

  this.isInit = ko.observable( false );

  // Summary UI methods and properties
  this.options = {
    customers: ko.observableArray([]),
    brokers: ko.observableArray([]),
    facilities: ko.observableArray([]),
    treatments: ko.observableArray([]),
    products: ko.observableArray([]),
    packaging: ko.observableArray([]),
    paymentTerms: ko.observableArray([]),
    shipmentMethods: ko.observableArray([])
  };

  this.summaryUI = {
    input: ko.observableArray([]),
    selected: ko.observable(),
    exports: ko.observable()
  };

  this.summaryUI.selected.subscribe(function( selectedItem ) {
    if ( selectedItem != null ) {
      page( '/' + selectedItem.QuoteNumber );
    }
  });

  // Summary pager
  var pagerOptions = {
    onNewPageSet: function() {
      self.summaryUI.input([]);
    }
  };

  var summaryPager = salesService.getQuotesDataPager( pagerOptions );

  this.summaryFilters = {
    customerKey: ko.observable(),
    brokerKey: ko.observable(),
    quoteDateStart: ko.observableDate(),
    quoteDateEnd: ko.observableDate(),
  };
  summaryPager.addParameters( this.summaryFilters );

  this.loadMore = ko.asyncCommand({
    execute: function( complete ) {
      summaryPager.nextPage()
      .done(function( data ) {
        var _input = self.summaryUI.input;

        ko.utils.arrayPushAll( _input, data );
        _input.notifySubscribers();
      }).always( complete );
    }
  });

  this.startNewQuote = ko.command({
    execute: function() {
      page('/new');
    }
  });

  // Editor UI methods and properties
  this.editorUI = {
    input: ko.observable(),
    options: this.options,
    exports: ko.observable()
  };

  this.isNew = ko.pureComputed(function() {
    var _editor = self.editorUI.input();

    return _editor && _editor.QuoteNumber == null;
  });

  this.isEditing = ko.pureComputed(function() {
    return self.editorUI.input();
  });

  this.isPickingAddress = ko.pureComputed(function() {
    var _editor = self.editorUI.exports();

    return _editor && _editor.isPickingAddress();
  });

  this.isEditorDirty = ko.pureComputed(function() {
    var _editor = self.editorUI.exports();

    return _editor && _editor.isDirty();
  });

  this.isEditorValid = ko.pureComputed(function() {
    var _editor = self.editorUI.exports();

    return _editor && _editor.isValid();
  });

  this.closeAddressPicker = ko.command({
    execute: function() {
      var _editor = self.editorUI.exports();

      return _editor && _editor.closeAddressPicker();
    }
  });

  this.salesQuoteReport = ko.pureComputed(function() {
      var _editor = self.editorUI.input();

      return _editor && _editor.Links && _editor.Links['report-sales-quote'].HRef;
  });

  this.isSaving = ko.observable( false );
  this.saveQuote = ko.asyncCommand({
    execute: function( complete ) {
      var _editor = self.editorUI.exports();
      if ( !_editor.isValid() ) {
        complete();
        showUserMessage( 'Please ensure all fields have been entered correctly' );
        return;
      }

      self.isSaving( true );

      var quoteData = _editor.toDto();
      var _isNew = quoteData.QuoteNumber == null;

      if ( !_isNew ) {
        var updateQuote = salesService.updateQuote( quoteData.QuoteNumber, quoteData ).then(
        function( data ) {
          self.summaryUI.exports().updateSummary( data );
          self.editorUI.input( data );
          self.editorUI.exports().resetDirtyFlag();
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not update quote', {
            description: errorThrown
          });
        })
        .always(function() {
          self.isSaving( false );
          complete();
        });

        return updateQuote;
      } else {
        var saveNewQuote = salesService.createQuote( quoteData ).then(
        function( data ) {
          self.summaryUI.exports().addSummary( data );
          self.editorUI.exports().resetDirtyFlag();
          page( '/' + data.QuoteNumber );
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not create new quote', {
            description: errorThrown
          });
        })
        .always(function() {
          self.isSaving( false );
          complete();
        });

        return saveNewQuote;
      }
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.isEditorDirty() && self.isEditorValid();
    }
  });

  this.closeQuote = ko.asyncCommand({
    execute: function( complete ) {
      if ( self.isEditorDirty() ) {
        showUserMessage( 'Save before closing?', {
          description: 'There are unsaved changes on the current quote. Would you like to save before closing?',
          type: 'yesnocancel',
          onYesClick: function() {
            if ( !self.saveQuote.canExecute() ) {
              showUserMessage( 'Unable to save quote' );

              return complete();
            }

            self.saveQuote.execute().then(function() {
              page('/');
            }).always( complete );
          },
          onNoClick: function() {
            self.editorUI.exports().resetDirtyFlag();
            page('/');
            complete();
          },
          onCancelClick: function() {
            complete();
          },
        });

        return;
      }

      page('/');
      complete();
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  // UI routing
  page.base('/Customers/Quotes');

  window.onbeforeunload = function(e) {
    if ( self.isEditorDirty() ) {
      var text = 'There are unsaved changed. Are you sure you want to leave?';

      e.returnValue = text;

      return text;
    }
  };

  var currentKey = null;
  var isNavigating = false;
  function checkDirtyBeforeNavigating( ctx, next ) {
    if ( isNavigating ) {
      isNavigating = false;

      return;
    }

    if ( self.isEditorDirty() ) {
      showUserMessage( 'Save before closing?', {
        description: 'There are unsaved changes on the current quote. Would you like to save before navigating?',
        type: 'yesnocancel',
        onYesClick: function() {
          if ( !self.saveQuote.canExecute() ) {
            showUserMessage( 'Unable to save quote' );
          }

          self.saveQuote.execute().then(function() {
            next();
          },
          function() {
            isNavigating = true;
            page( '/' + currentKey );
          });
        },
        onNoClick: function() {
          self.editorUI.exports().resetDirtyFlag();
          next();
        },
        onCancelClick: function() {
          isNavigating = true;
          page( '/' + currentKey );
        },
      });

      return;
    }

    next();
  }
  page( checkDirtyBeforeNavigating );

  var startQuote = function( ctx, next ) {
    var key = ctx.params.quoteID;

    if ( key === 'new' ) {
      self.editorUI.input( {} );

      return;
    }

    next();
  };
  page( '/:quoteID', startQuote );

  this.isLoadingDetails = ko.observable( false );
  var loadQuoteDetails = function( ctx, next ) {
    var key = ctx.params.quoteID;
    currentKey = key;

    if ( key != null ) {
      self.isLoadingDetails( true );
      var getDetails = salesService.getQuoteDetails( key )
      .done(function( data ) {
        self.editorUI.input( data );
      })
      .fail(function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not load quote', {
          description: errorThrown,
        });
        next();
      })
      .always(function() {
        self.isLoadingDetails( false );
      });

      return getDetails;
    }

    next();
  };
  page( '/:quoteID', loadQuoteDetails );

  function closeEditor() {
    self.editorUI.input( null );
  }

  var returnToSummaries = function() {
    closeEditor();
  };
  page( returnToSummaries );

  (function init() {
    var getCustomers = directoryService.getCustomers().then(
    function( data ) {
      self.options.customers( data );
    });

    var getBrokers = directoryService.getBrokers().then(
    function( data ) {
      self.options.brokers( data );
    });

    var getFacilities = warehouseService.getFacilities().then(
    function( data ) {
      self.options.facilities( data );
    });

    var getProducts = productsService.getChileProducts().then(
    function( data ) {
      self.options.products( data );
    });

    var getPackaging = productsService.getPackagingProducts().then(
    function( data ) {
      self.options.packaging( data );
    });

    var getTreatments = (function() {
      var treatments = Object.keys( rvc.lists.treatmentTypes ).map(function( type ) {
        return rvc.lists.treatmentTypes[ type ];
      });

      self.options.treatments( treatments );
    })();

    var getPaymentTerms = salesService.getPaymentTermOptions().then(
    function( data ) {
      self.options.paymentTerms( data );
    });

    var getShipmentMethods = salesService.getShipmentMethods().then(
    function( data ) {
      self.options.shipmentMethods( data );
    });


    var checkOptions = $.when( getCustomers, getBrokers, getFacilities, getProducts, getPackaging, getTreatments, getPaymentTerms, getShipmentMethods )
    .done(function() {
      self.isInit( true );
      page();
      self.loadMore.execute();
    })
    .fail(function() {
      showUserMessage( 'Could not start Quotes UI' );
    });

    return checkOptions;
  })();

  // Exports
  return this;
}

var vm = new QuotesVM();

ko.applyBindings( vm );

module.exports = vm;

