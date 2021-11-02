/* KO Components */
ko.components.register('receive-chile-editor', require('App/components/inventory/receive-chile-product-editor/receive-chile-product-editor'));
ko.components.register('receive-chile-summary', require('App/components/inventory/receive-chile-product-summary/receive-chile-product-summary'));
ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));

/* Required libraries */
var rvc = require('rvc');
var inventoryService = require('App/services/inventoryService');
var warehouseService = require('App/services/warehouseService');
var productsService = require('App/services/productsService');
var page = require('page');

require('App/koExtensions');
require('bootstrap');
require('App/helpers/koDeepValidationHelpers');

require('App/helpers/koPunchesFilters.js');
ko.punches.enableAll();

/* Receive Chile Product view model */
function ReceiveChileProductVM() {
  if ( !(this instanceof ReceiveChileProductVM) ) { return new ReceiveChileProductVM( params ); }

  var self = this;

  this.isInit = ko.observable( false );

  // Summary Component
  this.summaryData = {
    input: ko.observableArray( [] ),
    selected: ko.observable(),
    exports: ko.observable()
  };

  this.summaryData.selected.subscribe(function( item ) {
    if ( item ) {
      page( '/' + item.LotKey );
    }
  });

  // Summary items data pager
  this.filters = {
    materialsType: ko.observable(),
    startDate: ko.observable(),
    endDate: ko.observable(),
    supplierKey: ko.observable(),
    chileProductKey: ko.observable(),
  };

  var dataPager = inventoryService.getChileProductsDataPager({
    onNewPageSet: function() {
      self.summaryData.input( [] );
    }
  });
  dataPager.addParameters( this.filters );

  this.loadMore = ko.asyncCommand({
    execute: function( complete ) {
      return dataPager.nextPage().then(
      function( data, textStatus, jqXHR ) {
        ko.utils.arrayPushAll( self.summaryData.input(), data );
        self.summaryData.input.valueHasMutated();
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not load chile products', {
          description: errorThrown
        });
      }).always( complete );
    }
  });

  this.searchKey = ko.observable().extend({ lotKey: true });
  this.searchForKey = function () {
    var key = self.searchKey();

    loadDetailsByKey( key );
  };


  // Editor Component
  this.options = {
    materialTypes: [ { key: 0, value: "Dehydrated" }, { key: 1, value: "Other Raw" } ],
    chileProducts: ko.observable( {} ),
    packaging: ko.observableArray( [] ),
    suppliers: ko.observableArray( [] ),
    varieties: ko.observableArray( [] ),
    warehouses: ko.observableArray( [] ),
    warehouseLocations: ko.observable( {} )
  };

  this.editorData = {
    input: ko.observable(),
    options: this.options,
    exports: ko.observable()
  };

  this.editorSubtitleText = ko.pureComputed(function() {
    var _editorInput = self.editorData.input();
    var _editorExports = self.editorData.exports();

    var _lotKey = _editorInput && _editorInput.LotKey;
    var _materialType = _editorExports && _editorExports.materialsType();

    if ( _lotKey ) {
      return "" + _lotKey;
    } else {
      return _materialType === 1 ? 'Other Raw' : 'Dehydrated';
    }
  });

  this.isDirty = ko.pureComputed(function() {
    var _editorExports = self.editorData.exports();

    return _editorExports && _editorExports.isDirty();
  });

  this.addMaterials = ko.command({
    execute: function() {
    var _editorExports = self.editorData.exports();

    return _editorExports && _editorExports.addItemCommand.execute();
    }
  });

  this.numRowsToAdd = ko.observable( 1 );
  this.addMultipleMaterials = ko.command({
    execute: function() {
      var _rowsToAdd = self.numRowsToAdd();

      var i;
      for ( i = 0; i < _rowsToAdd; i++ ) {
        self.addMaterials.execute();
      }

      self.numRowsToAdd( 1 );
    },
    canExecute: function() {
      return self.numRowsToAdd() > 0;
    }
  });


  // Navigation Sidebar
  this.receiveDehydrated = ko.command({
    execute: function () {
      page('/newdehydrated');
    }
  });

  this.receiveOtherRaw = ko.command({
    execute: function () {
      page('/newother');
    }
  });

  function loadDetailsByKey( key ) {
    if ( key ) {
      page( '/' + key );
    }
  }

  this.recentEntries = ko.observableArray( [] );
  function addToRecentEntries( key ) {
    var _recentEntries = self.recentEntries();
    var _entryIndex = _recentEntries.indexOf( key );

    if ( _entryIndex > -1 ) {
      self.recentEntries.splice( _entryIndex, 1 );
    }

    self.recentEntries.unshift( key );

    if ( _recentEntries.length >= 5 ) {
      self.recentEntries.pop();
    }
  }

  this.recentEntryClicked = function ( vm, $element ) {
    var key = ko.contextFor( $element.target ).$data;

    if ( key ) {
      loadDetailsByKey( key );
    }
  };

  // Variety Editor
  this.newVarietyName = ko.observable();

  var $varietyModal = $('#new-variety-modal');
  $varietyModal.on('hidden.bs.modal', function() {
    self.newVarietyName( '' );
  });

  this.startNewVariety = ko.command({
    execute: function () {
      $varietyModal.modal('show');
    },
    canExecute: function () {
      return true;
    }
  });

  function addVariety( name ) {
    self.options.varieties.unshift( name );
  }

  function closeVarietyModal( Parameters ) {
    $varietyModal.modal('hide');
  }

  this.saveNewVariety = ko.command({
    execute: function () {
      var _name = self.newVarietyName();
      addVariety( _name );
      showUserMessage( 'New variety added', {
        description: 'The variety <b>' + _name + '</b> has been added.'
      });

      closeVarietyModal();
    },
    canExecute: function () {
      return self.newVarietyName();
    }
  });

  this.cancelNewVariety = ko.command({
    execute: function () {
      closeVarietyModal();
    },
    canExecute: function () {
      return true;
    }
  });

  // Product save tools
  this.saveCommand = ko.asyncCommand({
    execute: function ( complete ) {
      var _editor = self.editorData.exports();

      if ( !_editor.isValid() ) {
        showUserMessage( 'Could not save', {
          description: 'Please ensure all fields have been entered correctly.'
        });
        _editor.showErrors();

        complete();
        return;
      }

      var _entryData = _editor.toDto();
      var save;
      // If exists, update
      if ( _entryData.LotKey ) {
        save = inventoryService.updateChileProductReceivedRecord( _entryData.LotKey, _entryData ).then(
        function( data, textStatus, jqXHR ) {
          self.summaryData.exports().updateEntry( data );

          showUserMessage( 'Save successful', {
            description: 'You\'ve successfully updated the data for lot <b>' + _entryData.LotKey + '</b>.'
          });

          return data;
        });
      // Else, create new
      } else {
        save = inventoryService.createChileProductReceivedRecord( _entryData ).then(
        function( data, textStatus, jqXHR ) {
          self.summaryData.exports().addEntry( data );
          _editor.next();

          showUserMessage( 'Save successful', {
            description: 'You\'ve successfully created a new record.'
          });

          return data;
        });
      }

      var checkSave = save.then(
      function( data, textStatus, jqXHR ) {
        addToRecentEntries( data.LotKey );
        _editor.resetDirtyFlag();
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not save Chile Product request', {
          description: errorThrown
        });
      }).always( complete );

      return checkSave;
    },
    canExecute: function ( isExecuting ) {
      return !isExecuting && self.isDirty();
    }
  });

  this.closeEditorCommand = ko.command({
    execute: function () {
      if ( self.isDirty() ) {
        showUserMessage( 'Save changes?', {
          description: 'The current request has unsaved changes. Would you like to save?',
          type: 'yesnocancel',
          onYesClick: function() {
            self.saveCommand.execute().then(
            function( data, textStatus, jqXHR ) {
              page('/');
            });
          },
          onNoClick: function() {
            self.editorData.exports().resetDirtyFlag();

            page('/');
          },
          onCancelClick: function() { },
        });
      } else {
        page('/');
      }
    }
  });

  // Loads all data for UI before exposing it
  (function init() {
    var getChileProducts = productsService.getChileProducts().then(
    function( data, textStatus, jqXHR ) {
      self.options.chileProducts( data );
    });

    var getPackaging = productsService.getPackagingProducts().then(
    function( data, textStatus, jqXHR ) {
      self.options.packaging( data );
    });

    var getWarehouses = warehouseService.getWarehouses().then(
    function( data, textStatus, jqXHR ) {
      self.options.warehouses( data );
    });

    var _rinconKey = rvc.constants.rinconWarehouse.WarehouseKey;
    var getWarehouseLocations = warehouseService.getWarehouseLocations( _rinconKey ).then(
    function( data, textStatus, jqXHR ) {
      var _warehouseLocs = self.options.warehouseLocations();

      _warehouseLocs[ _rinconKey ] = data;

      self.options.warehouseLocations( _warehouseLocs );
    });

    var getSuppliers = inventoryService.getDehydrators().then(
    function( data, textStatus, jqXHR ) {
      self.options.suppliers( data );
    });

    var getChileVarieties = productsService.getChileVarieties().then(
    function( data, textStatus, jqXHR ) {
      self.options.varieties( data );
    });

    var checkOptions = $.when( getChileProducts, getPackaging, getWarehouses, getWarehouseLocations, getSuppliers, getChileVarieties ).then(
    function( data, textStatus, jqXHR ) {
      self.isInit( true );
      self.loadMore.execute();
      page();
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not load Receive Chile UI', {
        description: errorThrown
      });
    });
  })();

  // Routing
  page.base('/Warehouse/ReceiveChileProduct');

  function toDate(value) {
    var input = new Date( value );

    var dateStr = (input.getUTCMonth() + 1) + '/' + input.getUTCDate() + '/' + input.getUTCFullYear();

    return dateStr;
  }

  this.loadDetails = ko.asyncCommand({
    execute: function( key, complete ) {
      var _loadDetails = inventoryService.getChileMaterials( key ).then(
      function( data ) {
        data.DateReceived = toDate( data.DateReceived );

        self.editorData.input( data );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not get lot details', {
          description: errorThrown
        });
        page.redirect('/');
      }).always( complete );

      return _loadDetails;
    }
  });

  window.onbeforeunload = function(){
    if ( self.isDirty() ) {
      return 'Are you sure you want to leave?';
    }
  };

  var isRedirecting = false;
  var lastLocation = null;
  function checkIfDirty( ctx, next ) {
    if ( isRedirecting ) {
      isRedirecting = false;
      return;
    }

    if ( self.isDirty() ) {
      var _editor = self.editorData.input();
      var _lotKey = _editor && _editor.LotKey;

      showUserMessage( 'Save changes?', {
        description: 'The current request has unsaved changes. Would you like to save?',
        type: 'yesnocancel',
        onYesClick: function() {
          self.saveCommand.execute().then(
            function( data, textStatus, jqXHR ) {
            self.editorData.exports().resetDirtyFlag();
            lastLocation = ctx;
            next();
          },
          function( jqXHR, textStatus, errorThrown ) {
            isRedirecting = true;
            page( lastLocation.path );
          });
        },
        onNoClick: function() {
          self.editorData.exports().resetDirtyFlag();

          lastLocation = ctx;
          next();
        },
        onCancelClick: function() {
          isRedirecting = true;
          page( lastLocation.path );
        },
      });
    } else {
      lastLocation = ctx;
      next();
    }
  }
  page( checkIfDirty );

  function navigateToLot( ctx, next ) {
    var key = ctx.params.key;

    if ( key === 'newdehydrated' ) {
      self.editorData.input({
        LoadNumber: '1',
        ChileMaterialsReceivedType: 0
      });
    } else if ( key === 'newother' ) {
      self.editorData.input({
        LoadNumber: '1',
        ChileMaterialsReceivedType: 1
      });
    } else if ( key ) {
      var _loadDetails = self.loadDetails.execute( key ).then(
      function( data, textStatus, jqXHR ) {
        addToRecentEntries( key );
      });
    } else {
      next();
    }
  }
  page( '/:key', navigateToLot );

  function closeEditor() {
    self.summaryData.selected( null );

    var _editorExports = self.editorData.exports();
    if ( _editorExports ) {
      _editorExports.resetDirtyFlag();
    }

    self.editorData.input( null );

    closeVarietyModal();

    self.numRowsToAdd( 1 );
  }

  function navigateToSummary( ctx, next ) {
    closeEditor();
  }
  page( navigateToSummary );

  // Exports
  return this;
}

var vm = new ReceiveChileProductVM();

ko.applyBindings( vm );

module.exports = vm;
