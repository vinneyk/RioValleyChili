var templateMarkup = require('./inventory-by-lot-picker.html'),
    pickInventoryByLotModel = require('App/models/PickInventoryByLotModel');

require('App/koExtensions');
require('Scripts/knockout-projections.min');
require('App/scripts/ko.extenders.lotKey');
require('node_modules/knockout-jqautocomplete/build/knockout-jqAutocomplete');

function InventoryByLotViewModel (params) {
    if (!(this instanceof InventoryByLotViewModel)) return new InventoryByLotViewModel(params);

    // Init
    var self = this,
        config = ko.unwrap(params) || {},
            input = ko.unwrap(config.input) || {},
            _pickedInventoryItems = ko.observableArray(ko.utils.arrayMap(input.PickedInventoryItems || [], _mapPickedInventoryItem));

    // Initial validation checks
    if (!config.exports || !ko.isWritableObservable(config.exports)) {
        throw new Error("Missing or invalid initialization parameter: value. Expected an writable observable.");
    } else if (!config.warehouseLocationOptions || !ko.isObservable(config.warehouseLocationOptions)) {
        throw new Error("Missing or invalid parameter from the initialValues object: warehouseLocationOptions. Expected an observableArray object.");
    } else if (!config.inventoryProductOptions || !ko.isObservable(config.inventoryProductOptions)) {
        throw new Error("Missing or invalid parameter from the initialValues object: inventoryProductOptions. Expected an observableArray object.");
    } else if (!config.packagingProductOptions || !ko.isObservable(config.packagingProductOptions)) {
        throw new Error("Missing or invalid parameter from the initialValues object: packagingProductOptions. Expected an observableArray object.");
    }

    // Data
    this.isInit = ko.observable(false);

    this.warehouseLocationOptions = config.warehouseLocationOptions;
    this.inventoryProductOptions = config.inventoryProductOptions;
    this.packagingProductOptions = config.packagingProductOptions;
    this.PickedInventoryKey = ko.observable(input.PickedInventoryKey);

    // Computed data
    this.PickedInventoryItems = ko.pureComputed(function() {
        return _pickedInventoryItems();
    });
    this.filteredPickedInventoryItems = _pickedInventoryItems.filter(function (item) {
        return item && ko.unwrap(item.LotKey) && ko.unwrap(item.isPopulated);
    });
    this.isValid = function() {
        var items = ko.utils.arrayFilter(self.filteredPickedInventoryItems(), function (item) { 
            return item.QuantityPicked() != undefined; 
        });

        return ko.utils.arrayFirst(items, function(item) {
                return !item.isValid();
            }) == null;
    };

    this.TotalQuantityPicked = ko.pureComputed(function () {
        var sum = 0;

        ko.utils.arrayForEach(self.PickedInventoryItems() || [], function (item) {
            sum += item.QuantityPicked() || 0;
        });

        return sum;
    }, this);

    this.TotalQuantityPicked.formatted = ko.pureComputed(function () {
        return self.TotalQuantityPicked().toLocaleString();
    }, this);

    this.TotalWeightPicked = ko.pureComputed(function () {
        var sum = 0;

        ko.utils.arrayForEach(self.PickedInventoryItems() || [], function (item) {
            sum += item.WeightPicked() || 0;
        });

        return sum;
    }, this);

    // Subscriptions
    if (ko.isObservable(params.input)) {
        params.input.subscribe(function() {
            config = ko.unwrap(params) || {};
            input = ko.unwrap(config.input) || {};

            _pickedInventoryItems(ko.utils.arrayMap(input.PickedInventoryItems || [], _mapPickedInventoryItem));
        });
    }

    // Behaviors
    this.removePickedItemCommand = ko.command({
        execute: function (item) {
            ko.observableArray.fn.remove.call(_pickedInventoryItems, item);
        }
    });

    this.clearPickedItemsCommand = ko.command({
        execute: function() {
            _pickedInventoryItems([]);
        }
    });

    this.addPickedItemCommand = ko.command({
        execute: function () { addNewPickedItem(); }
    });

    function _addItem() {
        var picked = self.PickedInventoryItems();

        if (picked.length > 0) {
            for (var i = 0, list = picked, max = list.length; i < max; i += 1) {
                if (!list[i].LotKey()) {
                    return;
                }
            }
            self.addPickedItemCommand.execute();
        } else {
            return;
        }
    }

    function _mapPickedInventoryItem(input) {
        var mappedItem = new InventoryByLotViewModel.PickedInventoryItem(input, config);
        mappedItem.autoAddItem = _addItem;

        return mappedItem;
    }

    function _buildDirtyChecker() {
        var _initialized;

        this.isDirty = ko.computed(function() {
            if (!_initialized) {
                ko.toJS(_pickedInventoryItems);

                _initialized = true;

                return false;
            }

            return true;
        });
    }

    function _buildCache() {
        self.itemCache = ko.toJS(_pickedInventoryItems);
    }

    function addNewPickedItem(input) {
        _pickedInventoryItems.push(_mapPickedInventoryItem(input));
    }

    function saveData() {
        self.isInit(false);
        init();
    }
    function reset() {
        self.isInit(false);

        var items = ko.utils.arrayFilter(self.itemCache, function(item) {
          return item.InventoryKey;
        });

        var mappedItems = ko.utils.arrayMap(items, _mapPickedInventoryItem);
        if (mappedItems && mappedItems.length) {
          _pickedInventoryItems(ko.toJS(mappedItems));
        } else {
          _pickedInventoryItems([]);
          addNewPickedItem();
        }

        _buildDirtyChecker();

        self.isInit(true);
    }

    function init() {
        _buildCache();
        _buildDirtyChecker();
        self.isInit(true);
    }

    // Exports
    this.addNewPickedItem = addNewPickedItem;
    this.saveData = saveData;
    this.reset = reset;
    this.init = init;

    params.exports(this);

    init();

    return this;
}

ko.utils.extend(InventoryByLotViewModel, {
    dispose: function () {
        var self = this;
        ko.arrayForEach(self.PickedInventoryItems(), function (item) {
            item.dispose();
        });
    },

    PickedInventoryItem: pickInventoryByLotModel
});


module.exports = {
    viewModel: InventoryByLotViewModel,
    template: templateMarkup
};
