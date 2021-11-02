var productsService = require('App/services/productsService');

ko.punches.enableAll();

var rvc = require('App/rvc'),
    lotService = require('services/lotService'),
    inventoryService = require('App/services/inventoryService'),
    pickableInventoryFactory = require('App/models/PickableInventoryItem'),
    loadingMessage = require('App/components/common/loading-screen/loading-screen'),
    attributes = ko.observableArray([]),
    LotAttribute = require('App/models/LotAttribute');

var initAttributes = $.when(loadAttributeNames());

var inventoryPickingTableComponent = require('App/components/inventory/inventory-picking-table/inventory-picking-table');
// must be synchronous for sticky table headers to work.
inventoryPickingTableComponent.synchronous = true;
ko.components.register('inventory-picking-table', inventoryPickingTableComponent);

require('App/helpers/koValidationHelpers');
require('App/helpers/koPunchesFilters');
require('floatthead/dist/jquery.floatThead.js');

ko.punches.enableAll();

/**  Inputs:
  *
  * @param {Object} data - Container for input data
  * @param {Object} data.pickingContext
  * @param {string} data.pickingContextKey
  * @param {Object[]} data.pickedInventoryItems
  * @param {number} data.pageSize
  * @param {Object} data.filters
  * @param {Object} data.targetProduct
  * @param {Object} data.targetWeight
  * @param {string} data.customerKey
  * @param {string} data.customerLotCode
  * @param {string} data.customerProductCode
  * @param {string} data.args - optional object or observable to include additional properties to be included in the query parameters (for example, picking for sales order requires the inclusion of a contractItemKey)
  * @param {Function} [dataPager] - Optional pager to call instead of default
  * @param {function} [checkOutOfRange]
  * @param {boolean} [hideTheoretical=false] - Disables the calulcation and display of theoretical attrs
  * @param {Object} exports - Observable, container for exported methods and properties
  */

function InventoryPickerViewModel(params) {
  if (!(this instanceof InventoryPickerViewModel)) { return new InventoryPickerViewModel(params); }

  var self = this;
  var input = params.hasOwnProperty('data') ? params.data : params;
  var data = ko.unwrap(input);

  self.floatHeader = function () {
    var $tableSelector = $(arguments[0]).find('table');
    $tableSelector.floatThead({
      scrollContainer: function ($table) {
        return $table.closest('.sticky-head-container');
      }
    });
  };

  self.reflowTable = function () {
    var $table = $('table.sticky-head');
    $table.floatThead('reflow');
  };

  self.disposables = [];

  var targetProduct = data.targetProduct;
  self.targetProduct = targetProduct;
  self.targetWeight = data.targetWeight;

  self.hideTheoretical = params.hideTheoretical;

  self.isInit = ko.observable(false);
  self.isLoaded = ko.observable(false);
  self.isWorking = ko.observable(false);
  self.isSaving = ko.observable(false);

  self.loadingMessage = ko.observable('');

  self.pickingContext = data.pickingContext;
  self.pickingContextKey = data.pickingContextKey;
  self.otherArgs = data.args || {};
  self.customerKey = data.customerKey;
  self.customerLotCode = data.customerLotCode;
  self.customerProductCode = data.customerProductCode;

  self.targetProductName = ko.computed(function () {
    var product = ko.unwrap(targetProduct) || {};
    return product.ProductNameFull;
  });
  self.targetProductKey = ko.computed(function () {
    var product = ko.unwrap(targetProduct) || {};
    return product.ProductKey;
  });

  self.customerSpecs = ko.observable({});
  self.isCustomerSpecAvailable = ko.observable( false );
  function getCustomerProductSpecs( customerKey, productKey ) {
    self.customerSpecs({});
    var _targetProduct = ko.unwrap( targetProduct );
    var _productSpecs;
    if ( _targetProduct ) {
      _productSpecs = {};

      ko.utils.arrayForEach( _targetProduct.AttributeRanges, function( attrRange ) {
        _productSpecs[ attrRange.AttributeNameKey ] = attrRange;
      });
    }

    return productsService.getCustomerProductDetails( customerKey, productKey ).then(
    function( data ) {
      if ( data.length === 0 ) { return; }

      var specs = data.map(function( spec ) {
        spec.MinValue = spec.RangeMin;
        spec.MaxValue = spec.RangeMax;
        spec.AttributeNameKey = spec.AttributeShortName;
        spec.overridden = true;

        return spec;
      });

      var mappedSpecs = {};
      ko.utils.arrayForEach( specs, function( spec ) {
        mappedSpecs[ spec.AttributeNameKey ] = spec;
      });

      if ( _productSpecs ) {
        var mergedSpecs = $.extend({}, _productSpecs, mappedSpecs );

        self.customerSpecs( mergedSpecs );
      } else {
        self.customerSpecs( mappedSpecs );
      }

      self.isCustomerSpecAvailable( true );
    },
    function( jqXHR, textStatus, errorThrown ) {
      if ( jqXHR.status === 500 ) {
        showUserMessage( 'Could not get customer specs', {
          description: errorThrown
        });
      }
    });
  }

  self.isUsingCustomerSpec = ko.observable( false );

  if ( self.customerKey ) {
    getCustomerProductSpecs( ko.unwrap( self.customerKey ), self.targetProductKey() ).then(
    function( data, textStatus, jqXHR ) {
      self.isUsingCustomerSpec( true );
    });
  }
  function checkCustomerOutOfRange( key, value ) {
    var _customerSpecs = self.customerSpecs();

    var spec = _customerSpecs[ key ];

    if ( spec ) {
      if ( value < spec.MinValue ) {
        return -1;
      } else if ( value > spec.MaxValue ) {
        return 1;
      }
    }

    return 0;
  }

  self.checkOutOfRange = ko.pureComputed(function() {
    return self.isCustomerSpecAvailable() && self.isUsingCustomerSpec() ?
      checkCustomerOutOfRange :
      params.checkOutOfRange;
  });


  self.inventoryItems = ko.observableArray();
  var pickedInventoryItems = ko.computed(function() {
    return ko.utils.arrayFilter(self.inventoryItems(), function(i) {
      return i.isPicked() === true;
    });
  });

  self.isDirty = ko.pureComputed(function () {
    var picks = pickedInventoryItems();

    return ko.utils.arrayFirst(picks, function (item) {
      return ((item.isInitiallyPicked() &&
              item.isChanged()) ||
          (!item.isInitiallyPicked() &&
              item.isPicked() &&
              item.QuantityPicked() > 0));
    }) ? true : false;
  });
  self.currentInventoryType = ko.pureComputed(function () {
    var _input = ko.unwrap( input );
    var filters = ko.unwrap(_input.filters);
    return (filters && ko.unwrap(filters.inventoryType)) || rvc.lists.inventoryTypes.Chile.key;
  });

  self.showTable = function (table) {
    var tableType = table.inventoryType.key,
        targetInventoryType = self.currentInventoryType();

    if (tableType === targetInventoryType || targetInventoryType == undefined) {
      self.reflowTable();
      return true;
    }
    return false;
  };

  // commands
  self.saveCommand = ko.asyncCommand({
    execute: function (complete) {
      self.loadingMessage('Saving picked items');
      self.isSaving(true);

      try {
        var context = ko.unwrap(self.pickingContext),
            key = ko.unwrap(self.pickingContextKey),
            items = pickedInventoryItems(),
            values = [],
            args = ko.unwrap(self.otherArgs) || {};

        // Loops over inventory for picked items
        for (var i = 0, max = items.length; i < max; i += 1) {
          var item = items[i];
          if (!item.validation.isValid()) {
            showUserMessage('Failed to save items', { description: 'Please enter a valid quantity for all picked items' });
            complete();
            return;
          }

          var orderItemKey = args.orderItemKey || args.OrderItemKey;
          var isNewPick = item.OrderItemKey == null && orderItemKey != null;
          var inScopeForPicking = isNewPick || (args.orderItemKey != null && item.OrderItemKey === args.orderItemKey);

          if (item.QuantityPicked() > 0 && item.validation.isValid()) {
            values.push(ko.toJS({
              InventoryKey: item.InventoryKey,
              QuantityPicked: item.QuantityPicked,
              CustomerLotCode: inScopeForPicking ? ko.unwrap( self.customerLotCode ) : item.CustomerLotCode,
              CustomerProductCode: inScopeForPicking ? ko.unwrap( self.customerProductCode ) : item.CustomerProductCode,
              OrderItemKey: item.OrderItemKey || orderItemKey || null,
            }));
          } else {
            item.QuantityPicked(null);
          }
        }
      } catch (ex) {
        self.isSaving(false);
        complete();
        showUserMessage("An error occurred while attempting to save inventory. Please contact system administrator.", { description: 'Error description: ' + ex.message });
      }


      return inventoryService.savePickedInventory(context, key, values)
          .done(function () {
            ko.utils.arrayForEach(pickedInventoryItems(), function (i) {
              i.isInitiallyPicked(i.QuantityPicked() > 0);
              i.commit();
            });

            ko.postbox.publish('PickedItemsSaved', pickedInventoryItems());
            self.reflowTable();
            showUserMessage('Save successful', { description: 'Products have been successfully picked' });
          })
          .fail(function (promise, status, message) {
            showUserMessage('Failed to save items', { description: 'Server gave error: \n' + message });
          })
          .always(function () {
            complete();
            self.isSaving(false);
          });
    },
    canExecute: function (isExecuting) {
      return !isExecuting;
    }
  });
  self.revertCommand = ko.command({
    execute: function () {
      ko.utils.arrayForEach(pickedInventoryItems(), function (item) { item.revert(); });
      return true;
    },
    canExecute: function () {
      return true;
    }
  });

  var inventoryCache = {};

  // Behaviors
  function mapLotInventoryItemAsPickable(value) {
    return pickableInventoryFactory( value, self.checkOutOfRange );
  }
  function initializeInventoryCache() {
    var cache = {};
    ko.utils.arrayForEach(self.inventoryItems.peek(), function (item) {
      var cacheKey = ko.unwrap(item.Product.ProductType);
      if (!cache[cacheKey]) { cache[cacheKey] = {}; }
      cache[cacheKey][item.InventoryKey] = item;
    });
    inventoryCache = cache;
  }

  function buildDataPager(context, key, otherArgs) {
    var _input = ko.unwrap( input );
    var pagerOptions = {
      pageSize: _input.pageSize ? _input.pageSize : 50,
      parameters: $.extend({}, ko.unwrap(_input.filters), otherArgs || {}),
      onNewPageSet: function resetPickableArray() {
        self.inventoryItems(pickedInventoryItems());
        initializeInventoryCache();
      },
      onEndOfResults: function () {
        showUserMessage("All Inventory Loaded", { description: 'All inventory is loaded for the current filters. To view more inventory, change the filter selections on the right side of the page.' });
      }
    };

    return context && key ?
      inventoryService.getPickableInventoryPager(context, key, pagerOptions) :
      null;
  }

  var lotDataPager, contextHold, keyHold;
  function loadMoreItems() {
    var dfd = $.Deferred();
    self.isWorking(true);
    self.loadingMessage('Loading inventory...');

    var context = ko.unwrap(self.pickingContext),
        key = ko.unwrap(self.pickingContextKey);
    args = ko.unwrap(self.otherArgs);

    if (!lotDataPager || context !== contextHold || key !== keyHold) {
      lotDataPager = buildDataPager(context, key, args);
      contextHold = context;
      keyHold = key;
    }

    if (!lotDataPager) {
      console.log('Data pager is missing required parameters.');
      dfd.reject();
      return dfd;
    }

    lotDataPager.GetNextPage().done(function (values) {
      // Builds inventory item structure
      var cache = inventoryCache[self.currentInventoryType()];
      cache = cache || {};
      var mappedItems = [],
          cachedItem;

      ko.utils.arrayForEach(values, function (item) {
        cachedItem = cache[item.InventoryKey];
        if (cachedItem) return; //prevent duplicates

        cachedItem = mapLotInventoryItemAsPickable(item);
        //build cache (assumes all items are of the same inventory type)
        cache[item.InventoryKey] = cachedItem;
        mappedItems.push(cachedItem);
      });

      // Pushes data to master inventory list
      ko.utils.arrayPushAll(self.inventoryItems(), mappedItems);
      self.inventoryItems.valueHasMutated();
      self.isWorking(false);
      self.isLoaded(true);

      dfd.resolve();
    })
    .fail(function () {
      showUserMessage("Error loading inventory items.", { description: arguments[2] });
      dfd.reject();
      self.isWorking(false);
    });

    return dfd;
  }

  function mapInitiallyPickedItems(pickedItems) {
    var items = ko.unwrap(pickedItems) || [];
    var mappedItems = ko.utils.arrayMap(items, mapLotInventoryItemAsPickable);

    for (var i = 0, max = mappedItems.length; i < max; i += 1) {
      if (!mappedItems[i].isInitiallyPicked) {
        mappedItems[i].isInitiallyPicked = ko.observable(true);
      } else {
        mappedItems[i].isInitiallyPicked(true);
      }
    }
    return mappedItems;
  }

  function consolidateDuplicates(items) {
    var cache = {},
        hasError = false;

    var output = ko.utils.arrayFilter(items, function (item) {
      return !checkCache(item);
    });

    if (hasError) {
      showUserMessage('Consolidation error', { description: 'Attempted to consolidate multiple entries of same lot but quantity exceeded available product.' });
    }

    return output;

    function checkCache(item) {
      var cacheKey = item.InventoryKey,
          cacheItem = cache[cacheKey],
          qtyAvailable,
          diff;

      if (cacheItem) {
        qtyAvailable = cacheItem.QuantityAvailable.peek();

        diff = qtyAvailable - item.QuantityPicked.peek();
        if (diff < 0) {
          hasError = true;
        }

        cacheItem.QuantityPicked(cacheItem.QuantityPicked.peek() + item.QuantityPicked.peek());
        return true;
      } else {
        cache[cacheKey] = item;
        return false;
      }
    }
  }
  function setInitiallyPickedInventory() {
    var pickedItems = ko.unwrap(data.pickedInventoryItems) || [];

    ko.utils.arrayForEach(pickedItems, function (item) {
      var isAstaC = ko.utils.arrayFirst(item.Attributes, function (attr) {
        return attr.Key === "AstaC";
      });

      if (!isAstaC && item.AstaCalc) {
        item.Attributes.push( new LotAttribute({
          AttributeDate: item.Attributes[0].AttributeDate,
          Computed: false,
          Defect: undefined,
          Key: "AstaC",
          Name: "AstaC",
          Value: item.AstaCalc
        }) );
      }
    });

    if (!(pickedItems.length === 0 && self.inventoryItems.peek().length === 0)) {
      var consolidatedItems = consolidateDuplicates(mapInitiallyPickedItems(pickedItems));
      self.inventoryItems(consolidatedItems);
    }

    initializeInventoryCache();
  }

  if (ko.isObservable(data.pickedInventoryItems)) {
    var updateOnInputChange = data.pickedInventoryItems.subscribe(function (data) {
      setInitiallyPickedInventory();
    });

    self.disposables.push(updateOnInputChange);
  }
  setInitiallyPickedInventory();

  self.loadInventoryItemsCommand = ko.asyncCommand({
    execute: function (complete) {
      return loadMoreItems().always(complete);
    },
    canExecute: function (isExecuting) {
      return !isExecuting && self.isInit();
    }
  });

  initAttributes.then(function () {
    self.buildInventoryTables(targetProduct)
        .then(function () {
          self.isInit(true);
        });
  });

  self.errors = ko.pureComputed(function () {
    var errorArray = [];

    ko.utils.arrayForEach(self.inventoryItems(), function (item) {
      var productType = item.LotType.inventoryType.value;
      var isValid = item.validation.isValid();

      if (!isValid && errorArray.indexOf(productType) === -1) {
        errorArray.push(productType);
      }
    });

    return errorArray;
  });

  self.exports = ko.validatedObservable({
    saveCommand: self.saveCommand,
    revertCommand: self.revertCommand,
    isInit: self.isInit,
    isDirty: self.isDirty,
    isWorking: self.isWorking,
    isSaving: self.isSaving,
    loadInventoryItemsCommand: self.loadInventoryItemsCommand,
    pickedItems: self.inventoryItems
  });
  params.exports(self.exports);
}

InventoryPickerViewModel.prototype.buildInventoryTables = function (targetProduct) {
  var dfd = $.Deferred(),
      self = this;

  self.pickingTableViewModels = ko.observableArray([]);
  self.specWarnings = ko.observableArray([]);

  var tableVMs = [],
    exportedInventoryPickingTables = [];

  rvc.helpers.forEachInventoryType(function (inventoryType) {
    exportedInventoryPickingTables.push(ko.observable());
    tableVMs.push({
      inventoryType: inventoryType,
      inventoryItems: self.inventoryItems,
      targetProduct: targetProduct,
      useCustomerSpec: self.isUsingCustomerSpec,
      customerSpecs: self.customerSpecs,
      targetWeight: self.targetWeight,
      orderItemKey: ko.unwrap( self.otherArgs ).orderItemKey || null,
      attributes: attributes()[inventoryType.key] || [],
      exports: exportedInventoryPickingTables[exportedInventoryPickingTables.length - 1]
    });
  });

  ko.utils.arrayPushAll(self.pickingTableViewModels(), tableVMs);
  self.pickingTableViewModels.valueHasMutated();

  self.specWarnings = ko.pureComputed(function() {
    var warnings = [];
    ko.utils.arrayForEach(exportedInventoryPickingTables, function(pickTableExports) {
      var pickTable = pickTableExports();
      if (pickTable == undefined) return;
      var oos = pickTable.attributesOutOfSpec();
      if (oos.length) ko.utils.arrayPushAll(warnings, oos);
    });

    return warnings;
  });

  dfd.resolve();
  return dfd;
};

InventoryPickerViewModel.prototype.events = {
  pickedItemsSaved: 'PickedInventorySaved'
};

function selfOrObservable(candidate) {
  return ko.isObservable(candidate) ? candidate : ko.observable(candidate);
}
function selfOrObservableArray(candidate) {
  return ko.isObservable(candidate) ? candidate : ko.observableArray(candidate);
}

function loadAttributeNames() {
  if (initAttributes) {
    return initAttributes;
  }

  this.loadAttributeNamesAttempts = this.loadAttributeNamesAttempts || 0;
  this.loadAttributeNamesAttempts++;
  var me = this;

  return lotService.getAttributeNames()
      .done(function (data) {
        attributes(data);
      })
      .fail(function (xhr, result, message) {
        if (me.loadAttributeNamesAttempts < 5) me.loadAttributeNames();
        else showUserMessage('Failed to get attribute name values.', { description: 'There was a problem loading attribute names. Please notify system administrator with the following error message: "' + message + '".', type: 'error' });
      });
}

InventoryPickerViewModel.prototype.dispose = function () {
  ko.utils.arrayForEach(this.disposables, this.disposeOne);
  ko.utils.objectForEach(this, this.disposeOne);
};

InventoryPickerViewModel.prototype.disposeOne = function (propOrValue, value) {
  var disposable = value || propOrValue;

  if (disposable && typeof disposable.dispose === "function") {
    disposable.dispose();
  }
};

module.exports = {
  viewModel: InventoryPickerViewModel,
  template: require('./inventory-picker.html')
};
