require('App/helpers/koPunchesFilters.js');
ko.punches.enableAll();

var rvc = require('rvc');
var inventoryService = require('App/services/inventoryService');
var lotService = require('App/services/lotService');
var PickableInventoryItem = require('App/models/PickableInventoryItem');

require('floatthead/dist/jquery.floatThead.js');

ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
ko.components.register( 'lot-filters', require('App/components/common/lot-filters/lot-filters') );

var inventoryPickingTableComponent = require('App/components/inventory/inventory-picking-table/inventory-picking-table');
// must be synchronous for sticky table headers to work.
inventoryPickingTableComponent.synchronous = true;

ko.components.register( 'inventory-table', inventoryPickingTableComponent );

function InventoryVM() {
  if ( !(this instanceof InventoryVM) ) { return new InventoryVM(); }

  var self = this;

  // Data
  this.isInit = ko.observable(false);

  var _inventoryPager;
  var _buildingPager = false;

  var _attributes = ko.observable(  );

  var _inventoryItems = ko.observableArray( [] );

  this.filters = {
    filters: ko.observable( null ),
    exports: ko.observable( null )
  };

  var pagerOpts = {
    onNewPageSet: function() {
      _inventoryItems( [] );
      self.loadPageCounter( 1 );
    },
    onEndOfResults: function () {
      showUserMessage( "All Inventory Loaded", {
        description: 'All inventory is loaded for the current filters. To view more inventory, change the filter selections on the right side of the page.'
      } );
    },
    pageSize: 50
  };

  this.tableVMs = ko.observableArray( [] );

  this.currentInventoryType = ko.pureComputed(function () {
    var filters = ko.unwrap( self.filters.filters );

    return (filters && ko.unwrap( filters.inventoryType )) || rvc.lists.inventoryTypes.Chile.key;
  });

  // Behaviors
  var continueLoading = false;
  this.loadPageCounter = ko.observable( 1 );
  function getNextPage() {
    var getNext = _inventoryPager.GetNextPage().then(
      function( data ) {
      var newData = data.Items || [];

      //continueLoading = false;
      //if ( newData.length >= pagerOpts.pageSize ) {
      //  continueLoading = true;
      //}
      self.totalPounds(data.TotalPounds || 0);

      return newData;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Load failed', { description: errorThrown } );
    });

    var mapNewData = getNext.then(
      function (data) {
      var newData = ko.utils.arrayMap( data, function( item ) {
        var mappedItem = new PickableInventoryItem( item, checkOutOfRange );

        function checkOutOfRange( key, value ) {
          var defect = this.Defect;
          var resolution = defect && ko.unwrap( defect.Resolution );

          if ( defect && !resolution ) {
            var max = defect.AttributeDefect.OriginalMaxLimit;

            return value > max ? 1 : -1;
          } else {
            return 0;
          }
        }

        return mappedItem;
      });

      ko.utils.arrayPushAll(_inventoryItems(), newData);
      _inventoryItems.notifySubscribers(_inventoryItems());

      return newData;
    });

    return mapNewData;
  }

  this.enableAutoLoad = ko.observable( true );
  this.isLoadingMultiple = ko.observable(false);
  this.totalPounds = ko.observable(0);

  function getNextPageSuccessCallback() {
    self.loadPageCounter( +self.loadPageCounter() + 1 );

    if ( self.enableAutoLoad() && continueLoading ) {
      self.isLoadingMultiple( true );
      getNextPage().done( getNextPageSuccessCallback );
    } else {
      self.isLoadingMultiple( false );
    }
  }

  this.loadMore = ko.asyncCommand({
    execute: function( complete ) {
      return getNextPage().done( getNextPageSuccessCallback ).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.isInit();
    }
  });

  this.floatHeader = function () {
    var $tableSelector = $(arguments[0]).find('table');
    $tableSelector.floatThead({
      scrollContainer: function ($table) {
        return $table.closest('.sticky-head-container');
      }
    });
  };

  this.reflowTable = function () {
    var $table = $('table.sticky-head');
    $table.floatThead('reflow');
  };

  this.showTable = function (table) {
    var tableType = table.inventoryType.key;
    var targetInventoryType = self.currentInventoryType();

    if (tableType === targetInventoryType || targetInventoryType == null) {
      self.reflowTable();
      return true;
    }
    return false;
  };

  function loadAttributeNames() {
    var getAttrs =  lotService.getAttributeNames().then(
    function( data ) {
      _attributes(data);
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage('Failed to get attribute name values.', { description: 'There was a problem loading attribute names. Please notify system administrator with the following error message: "' + errorThrown + '".', type: 'error' });
    });

    return getAttrs;
  }

  function buildTableVMs() {
    rvc.helpers.forEachInventoryType(function ( inventoryType ) {
      var inventoryExport = ko.observable();
      self.tableVMs.push({
        inventoryType: inventoryType,
        inventoryItems: _inventoryItems,
        attributes: _attributes()[inventoryType.key] || [],
        visible: ko.pureComputed(function() {
          var filters = self.filters.filters();
          var filterKey = filters && filters.inventoryType();
          return inventoryType.key === filterKey;
        }),
        inventoryTableExport: inventoryExport,
        totalPoundsOnScreen: ko.pureComputed(function() {
          var inventory = inventoryExport();
          if (inventory == null) {
            return 0;
          }
          return inventory.totalPoundsOnScreen();
        })

      });
    });
  }

  function buildPager() {
    _inventoryPager = inventoryService.getInventoryPagerWithTotals( pagerOpts );
    _inventoryPager.addParameters( self.filters.filters() );
  }

  function init() {
    var loadAttrs = loadAttributeNames().then(
    function( data, textStatus, jqXHR ) {
      return data;
    });

    var buildVmData = loadAttrs.then(function( data ) {

      var filterSub = ko.computed({
        read: function() {
          var filters = self.filters.filters();

          if ( filters && !_buildingPager && !_inventoryPager ) {
            _buildingPager = true;
            buildPager();
            buildTableVMs();
          }

          if ( _inventoryPager ) {
            self.isInit( true );
          }
        },
        disposeWhen: function() {
          return self.isInit();
        }
      });
    });
  }

  init();

  // Exports
  return this;
}

var vm = new InventoryVM();

ko.applyBindings( vm );

module.exports = vm;

