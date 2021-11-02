/** Required libraries */
var inventoryAdjustmentsService = require('App/services/inventoryAdjustmentsService');
var lotService = require('App/services/lotService');
var productsService = require('App/services/productsService');
var warehouseService = require('App/services/warehouseService');
var warehouseLocationsService = require('App/services/warehouseLocationsService');

var page = require('page');

/** KO Punches filters */
require('App/helpers/koPunchesFilters');
ko.punches.enableAll();

/** Knockout Components */
ko.components.register( 'lot-filters', require('App/components/common/lot-filters/lot-filters'));
ko.components.register( 'inventory-adjustments-summary', require('App/components/inventory/inventory-adjustments-summary/inventory-adjustments-summary'));
ko.components.register( 'inventory-adjustments-editor', require('App/components/inventory/inventory-adjustments-editor/inventory-adjustments-editor'));

/** Inventory Adjustments ViewModel */
function InventoryAdjustmentsVM() {
  if ( !(this instanceof InventoryAdjustmentsVM) ) { return new InventoryAdjustmentsVM( params ); }

  var self = this;
  this.isInit = ko.observable( false );
  this.isRedirecting = ko.observable( false );
  var initLoad = true;
  var _isClosing = false;

  // Data
  this.filtersData = {
    filters: ko.observable(  )
  };

  var summaryItems = ko.observableArray( [] );
  this.currentKey = ko.observable(  );
  this.searchKey = ko.observable( null ).extend({ });

  this.summaryData = {
    input: summaryItems,
    selected: ko.observable( null )
  };

  this.editorData = {
    input: ko.observable(  ),
    options: {
      facilities: ko.observableArray( [] ),
      packaging: ko.observableArray( [] ),
      locations: ko.observableArray( [] ),
      treatments: (ko.observable().extend({ treatmentType: true })).options
    },
    exports: ko.observable(  )
  };

  var _inventoryPager = inventoryAdjustmentsService.getInventoryAdjustmentsPager({
    pageSize: 20,
    onNewPageSet: function() {
      summaryItems( [] );
    }
  });

  this.isEditorDirty = ko.pureComputed(function() {
    var editorExports = self.editorData.exports();

    return editorExports && editorExports.dirtyFlag.isDirty();
  });
  this.isNew = ko.pureComputed(function() {
    return self.editorData.input() === 'new';
  });

  // Behaviors
  var getDetails = ko.asyncCommand({
    execute: function( key, complete ) {
      var getDetailsForKey = inventoryAdjustmentsService.getInventoryAdjustment( key ).then(
      function( data, textStatus, jqXHR ) {
        return data;
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not find key', {
          description: errorThrown
        });
      });

      var mapDetails = getDetailsForKey.then(
      function( data ) {
        self.editorData.input( data );
      }).always( complete );

      return mapDetails;
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.startNewAdjustment = ko.command({
    execute: function() {
      page('/new');
    },
    canExecute: function() {
      return self.isInit();
    }
  });

  this.searchForKey = ko.asyncCommand({
    execute: function( complete ) {
      var key = self.searchKey();

      page( '/' + key );
      complete();
    },
    canExecute: function( isExecuting ) {
      var key = self.searchKey();

      return !isExecuting && key != null && key !== '' && getDetails.canExecute();
    }
  });

  this.saveDetailsCommand = ko.asyncCommand({
    execute: function( complete ) {
      var data = self.editorData.exports().toDto();

      if ( !data ) {
        showUserMessage( 'Could not create adjustments', {
          description: 'Please ensure all required fields are filled out and valid.'
        });

        complete();
        return;
      }

      var saveDetails = inventoryAdjustmentsService.postInventoryAdjustment( data ).then(
      function( data, textStatus, jqXHR ) {
        // Append to summary table
        summaryItems.unshift( data );

        // Notify user of successful save
        showUserMessage( 'Save successful', {
          description: 'A new adjustment has been created with the key <b>' + data.InventoryAdjustmentKey + '</b>.'
        });

        self.editorData.exports().dirtyFlag.reset();

        // Navigate to details page
        if ( !_isClosing ) {
          page( '/' + data.InventoryAdjustmentKey );
        } else {
          _isClosing = false;
        }
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Save failed', {
          description: errorThrown
        });
      }).always( complete );

      return saveDetails;
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.closeDetailsCommand = ko.asyncCommand({
    execute: function( complete ) {
      var isDirty = self.isEditorDirty();

      if ( isDirty ) {
        showUserMessage( 'Save before closing?', {
          description: 'There are unsaved changes on the current adjustment. Did you want to save before closing?',
          type: 'yesnocancel',
          onYesClick: function() {
            _isClosing = true;
            self.saveDetailsCommand.execute().then(
            function( data, textStatus, jqXHR ) {
              self.editorData.exports().dirtyFlag.reset();
              page('/');
            }).always( complete );
          },
          onNoClick: function() {
            self.editorData.exports().dirtyFlag.reset();
            page('/');
            complete();
          },
          onCancelClick: function() {
            complete();
          }
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

  function getNextPage() {
    var getNext = _inventoryPager.GetNextPage().then(
    function( data, textStatus, jqXHR ) {
      return data;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get inventory adjustments', {
        description: errorThrown
      } );
    });

    var mapData = getNext.then(function( data ) {
      return summaryItems( summaryItems().concat( data ) );
    });

    return mapData;
  }

  this.loadMore = ko.asyncCommand({
    execute: function( complete ) {
      var getNext = getNextPage().then(
      function( data ) {
        // Done
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.summaryData.selected.subscribe(function( invAdjustment ) {
    if ( invAdjustment && invAdjustment.InventoryAdjustmentKey ) {
      page( '/' + invAdjustment.InventoryAdjustmentKey );
    }
  });

  function loadProductOptions() {
    var productDfds = [];
    var options = {
      filterProductsWithInventory: false
    };

    rvc.helpers.forEachLotType( loadAndPush );

    var checkResults = $.when.apply( $, productDfds );

    function loadAndPush(type) {
      if (!ko.isObservable( productOptionsByLotType[ type.value ] )) {
        productOptionsByLotType[ type.value ] = ko.observableArray( [] );
      }

      var dfd = lotService.getProductsByLotType( type.key, options )
          .done(function (data) {
            productOptionsByLotType[type.value](data);
          });

      productDfds.push(dfd);
      return dfd;
    }

    return checkResults;
  }

  function loadOptions() {
    var opts = self.editorData.options;

    var getFacilities = warehouseService.getWarehouses().then( null );
    var getLocations = warehouseLocationsService.getRinconWarehouseLocations().then( null );
    var getPackaging = productsService.getPackagingProducts().then( null );

    var checkComplete = $.when( getFacilities, getLocations, getPackaging );

    return checkComplete;
  }

  var filtersSubscription = self.filtersData.filters.subscribe(function( filters ) {
    if ( filters ) {
      filtersSubscription.dispose();

      _inventoryPager.addParameters( filters );

      var getOptions = loadOptions().then(
      function( facilities, locations, packaging ) {
        self.editorData.options.facilities( facilities[0] || [] );
        self.editorData.options.locations( { 2: locations[0] } || [] );
        self.editorData.options.packaging( packaging[0] || [] );

        /** Enable the VM */
        self.isInit( true );
        // page( '/new' );
        page();

        /** setTimeout allows filter parameters to populate */
        setTimeout(function() {
          self.loadMore.execute();
        });
      });
    }
  });

  page.base('/Warehouse/InventoryAdjustments');
  page( '*', checkIfDirty );
  page( '/:key?', navigateToKey );
  window.onbeforeunload = function() {
    var isDirty = self.isEditorDirty();

    if ( isDirty ) {
      return "You have unsaved changes. Are you sure you want to leave the page?";
    }
  };

  function checkIfDirty( ctx, next ) {
    var currentKey = self.currentKey();

    if ( self.isRedirecting() ) {
      self.isRedirecting( false );

      return;
    }

    if ( self.editorData.input() && self.isEditorDirty() ) {
      showUserMessage( 'Save before navigating?', {
        description: 'There are unsaved changes on the current adjustment. Did you want to save before navigating?',
        type: 'yesnocancel',
        onYesClick: function() {
          _isClosing = true;
          self.saveDetailsCommand.execute().then(
            function( data, textStatus, jqXHR ) {
            next();
          },
          function( jqXHR, textStatus, errorThrown ) {
            showUserMessage( 'Save failed', { description: errorThrown } );
            self.isRedirecting( true );
            page( '/' + currentKey );
          });
        },
        onNoClick: function() {
          next();
        },
        onCancelClick: function() {
          self.isRedirecting( true );
          page( '/' + currentKey );
        }
      });
    } else {
      next();
    }
  }

  function navigateToKey( ctx ) {
    var currentKey = self.currentKey();
    var nextKey = ctx.params.key;
    var _isInitLoad = initLoad;
    initLoad = false;

    self.currentKey( nextKey );

    if ( !nextKey ) {
      self.editorData.input( null );
      self.editorData.exports( null );

      if ( _isInitLoad ) {
        page.redirect( '/new' );
      }
    } else if ( nextKey === 'new' ) {
      self.editorData.input('new');
    } else if ( nextKey ) {
      var getDetailsForKey = getDetails.execute( nextKey );

      return getDetailsForKey;
    }
  }

  // Exports
  return this;
}

var vm = new InventoryAdjustmentsVM();

ko.applyBindings( vm );

module.exports = vm;

