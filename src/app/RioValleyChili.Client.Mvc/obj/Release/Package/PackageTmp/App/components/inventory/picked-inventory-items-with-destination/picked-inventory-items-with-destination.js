var templateMarkup = require('./picked-inventory-items-with-destination.html'),
    inventoryService = require('services/inventoryService'),
    helpers = require('App/helpers/koHelpers');

require('Scripts/sh.knockout.observableWithLookup');
require('App/koExtensions');
require('Scripts/knockout-projections.min');
require('App/scripts/ko.extenders.lotKey');
require('node_modules/knockout-jqautocomplete/build/knockout-jqAutocomplete');

var InventoryPickOrderViewModel = function (params) {
    if (!(this instanceof InventoryPickOrderViewModel)) return new InventoryPickOrderViewModel(params);

    // Init
    var model = this,
        config = ko.unwrap(params) || {},
        input = ko.unwrap(config.input) || {},
        _pickedInventoryItems = ko.observableArray(ko.utils.arrayMap(input.PickedInventoryItems || [], mapPickedInventoryItem));

    // Initial validation checks
    if (!config.exports || !ko.isWritableObservable(config.exports)) throw new Error("Missing or invalid initialization parameter: value. Expected an writable observable.");
    if (!config.warehouseLocationOptions || !ko.isObservable(config.warehouseLocationOptions)) throw new Error("Missing or invalid parameter from the initialValues object: warehouseLocationOptions. Expected an observableArray object.");
    if (!config.inventoryProductOptions || !ko.isObservable(config.inventoryProductOptions)) throw new Error("Missing or invalid parameter from the initialValues object: inventoryProductOptions. Expected an observableArray object.");
    if (!config.packagingProductOptions || !ko.isObservable(config.packagingProductOptions)) throw new Error("Missing or invalid parameter from the initialValues object: packagingProductOptions. Expected an observableArray object.");

    // Data
    this.warehouseLocationOptions = config.warehouseLocationOptions;
    this.inventoryProductOptions = config.inventoryProductOptions;
    this.packagingProductOptions = config.packagingProductOptions;

    this.PickedInventoryKey = ko.observable(input.PickedInventoryKey);
    this.isEditable = params.isEditable;
    this.locationDescription = function (item) {
        return item.Location.Description;
    };

    this.PickedInventoryItems = ko.pureComputed(function() {
        return _pickedInventoryItems();
    });
    this.filteredPickedInventoryItems = _pickedInventoryItems.filter(function (item) {
            return item.LotKey() && item.isPopulated();
        });

    this.isValid = function() {
        var items = ko.utils.arrayFilter(model.filteredPickedInventoryItems(), function (item) { return item.QuantityPicked() != undefined; });
        return items.length > 0 && 
            ko.utils.arrayFirst(items, function(item) {
                return !item.isValid();
            }) == null;
    };

    // Behaviors
    function addItem() {
        var picked = model.PickedInventoryItems();

        if (picked.length > 0) {
            for (var i = 0, list = picked, max = list.length; i < max; i += 1) {
                if (!list[i].LotKey()) {
                    return;
                }
            }
            model.addPickedItemCommand.execute();
        } else {
            return;
        }
    }
    function mapPickedInventoryItem(input) {
        var mappedItem = new InventoryPickOrderViewModel.PickedInventoryItem(input, config);
        mappedItem.autoAddItem = addItem;

        return mappedItem;
    }

    function addNewPickedItem(input) {
        _pickedInventoryItems.push(mapPickedInventoryItem(input));
    }

    //#region commands
    this.removePickedItemCommand = ko.command({
        execute: function (item) {
            ko.observableArray.fn.remove.call(_pickedInventoryItems, item);
        },
        canExecute: function () { return model.isEditable(); }
    });
    this.clearPickedItemsCommand = ko.command({
        execute: function() {
            _pickedInventoryItems([]);
        }
    });
    this.addPickedItemCommand = ko.command({
        execute: function () { addNewPickedItem(); },
        canExecute: function () { return model.isEditable(); }
    });
    //#endregion commands

    //#region computed properties
    this.TotalQuantityPicked = ko.pureComputed(function () {
        var sum = 0;

        ko.utils.arrayForEach(model.PickedInventoryItems() || [], function (item) {
            sum += item.QuantityPicked() || 0;
        });

        return sum;
    }, this);

    this.TotalQuantityPicked.formatted = ko.pureComputed(function () {
        return model.TotalQuantityPicked().toLocaleString();
    }, this);

    this.TotalWeightPicked = ko.pureComputed(function () {
        var sum = 0;

        ko.utils.arrayForEach(model.PickedInventoryItems() || [], function (item) {
            sum += item.WeightPicked() || 0;
        });

        return sum;
    }, this);

    this.TotalWeightPicked.formatted = ko.pureComputed(function () {
        return model.TotalWeightPicked().toLocaleString();
    }, this);
    //#endregion

    
    // Manual subscriptions
    params.input.subscribe(function() {
        config = ko.unwrap(params) || {};
        input = ko.unwrap(config.input) || {};
        _pickedInventoryItems(ko.utils.arrayMap(input.PickedInventoryItems || [], mapPickedInventoryItem));
    });

    // Exported values
    params.exports(this);

    model.addNewPickedItem = addNewPickedItem;

    return model;
};

ko.utils.extend(InventoryPickOrderViewModel, {
    dispose: function () {
        var model = this;
        ko.arrayForEach(model.PickedInventoryItems(), function (item) {
            item.dispose();
        });
    },

    PickedInventoryItem: function (input, config) {
        if (!config || !config.warehouseLocationOptions) throw new Error('Missing configuration setting: warehouseLocationOptions');
        if (!(this instanceof InventoryPickOrderViewModel.PickedInventoryItem)) return new InventoryPickOrderViewModel.PickedInventoryItem(input, config);

        // Init
        var values = input || {},
            model = this,
            quantityAvailable = ko.observable(0),
            getInventoryDeferred,
            warehouseLocationSubscription;

        // Data
        this.QuantityPicked = ko.numericObservable(values.QuantityPicked);
        this.LotKey = ko.observable(values.LotKey).extend({ lotKey: searchLotKey });
        this.InventoryKey = ko.observable().extend({ required: { onlyIf: function () { return model.LotKey() != undefined; }}});
        this.isPopulated = ko.observable(false);

        this.OriginWarehouseLocation = ko.observableWithLookup({
            value: values.Location,
            keyDelegate: function (opt) { return opt.LocationKey; },
            displayProjector: function (selected) { return selected.Description; },
            lookupOptions: config.warehouseLocationOptions
        });
        this.OriginWarehouseLocationsWithInventory = ko.observableArray([]);

        // TODO: observableWithLookup marked for death. Seek and Destroy - VK 8/23/2015
        //this.DestinationWarehouseLocation = ko.observableWithLookup({
            //value: values.DestinationLocation.Description,
            //keyDelegate: function (opt) { return opt.LocationKey; },
            //optionValueDelegate: function (opt) { return opt.LocationKey; },
            //displayProjector: function (selected) { return selected.Description; },
            //lookupOptions: config.warehouseLocationOptions
        //}).extend({ notify: 'always', required: { onlyIf: function () { return model.QuantityPicked() != undefined; }} });
        this.DestinationWarehouseLocationValue = ko.observable(values.DestinationLocation? values.DestinationLocation.Description : null).extend({ notify: 'always', required: { onlyIf: function () { return model.QuantityPicked() != undefined; }} });
        this.DestinationWarehouseLocationOptions = ko.pureComputed(function () {
            var options = config.warehouseLocationOptions() || [];
            return ko.utils.arrayMap(options, function(item) {
                return item.Description;
            });
        }, this);
        this.isValidDestination = ko.pureComputed(function() {
            var value = this.DestinationWarehouseLocationValue(),
                lot = this.LotKey();

            return !lot || (lot && value && this.DestinationWarehouseLocationOptions().indexOf(value) > -1);
        }, this);
        this.DestinationWarehouseLocation = ko.pureComputed(function() {
            var description = this.DestinationWarehouseLocationValue(),
                options = config.warehouseLocationOptions(),
                matchingDestination = this.isValidDestination() ? ko.utils.arrayFirst(options, function(option) {
                return option.Description === description;
            }) :
                null;

            return matchingDestination || {};
        }, this).extend({ required: { onlyIf: function () { return model.QuantityPicked() != undefined; }} });;
        this.enableDestinationWarehouseLocation = ko.pureComputed(function () {
            return this.DestinationWarehouseLocationOptions().length > 0;
        }, this);

        this.originWarehouseLocationHasFocus = ko.observable();
        this.packagingProductOptionsHasFocus = ko.observable();
        this.treatmentOptionsHasFocus = ko.observable();

        this.InventoryProductName = ko.observable(values.Product && values.Product.ProductCodeAndName);

        if (values.Product) {
            values.PackagingProductKey = values.PackagingProduct.ProductKey;
            values.TreatmentKey = values.InventoryTreatment.TreatmentKey;
            values.WarehouseLocationKey = values.Location.LocationKey;
        }

        this.InventoryForLot = ko.observableArray(values.Product ? 
            [values] :
            []
        );

        this.PackagingProduct = ko.observableWithLookup({
            value: values.PackagingProduct,
            keyDelegate: function (opt) { return opt && opt.ProductKey; },
            displayProjector: function (selected) { return selected.ProductName; },
            lookupOptions: config.packagingProductOptions
        });
        this.PackagingProductOptions = ko.pureComputed(function () {
            var options = ko.unwrap(this.PackagingProduct.options),
                availableInventory = model.InventoryForLot(), 
                availableOptions = (function optionsAtLocation () {
                    var availablePackagingKeys = [],
                        validPackagingOptions = [],
                        cachedIndex = 0;

                    ko.utils.arrayForEach(availableInventory, function (item) {
                        availablePackagingKeys.push(item.PackagingProduct.ProductKey);
                    });

                    for (var i = 0, list = options, max = list.length; i < max;
                         i += 1) {
                        cachedIndex = availablePackagingKeys.indexOf(list[i].ProductKey);

                        if (cachedIndex > -1) {
                            validPackagingOptions.push(list[i]);
                            availablePackagingKeys.splice(cachedIndex, 1);

                            if (availablePackagingKeys.length === 0) {
                                break;
                            }
                        }
                    }
                    return validPackagingOptions;
                })();

            return availableOptions;
        }, this);

        this.InventoryTreatment = ko.observable(values.InventoryTreatment && values.InventoryTreatment.TreatmentKey)
            .extend({ treatmentType: true });

        this.WeightPicked = ko.pureComputed(function () {
            var packaging = this.PackagingProduct();
            return (packaging ? packaging.Weight : 0) * (this.QuantityPicked() || 0);
        }, this);

        this.WeightPicked.formatted = ko.pureComputed(function () {
            return this.WeightPicked().toLocaleString();
        }, this);

        this.InitialQuantityPicked = ko.computed(function () {
            var lotKey = model.LotKey(),
                packagingProduct = model.PackagingProduct(),
                treatment = model.InventoryTreatment(),
                location = model.OriginWarehouseLocation();

            if (lotKey == undefined || packagingProduct == undefined || treatment == undefined || location == undefined) return 0;

            return (values.LotKey === lotKey &&
                    values.PackagingProduct.ProductKey === (typeof packagingProduct === "string" ? packagingProduct : packagingProduct.ProductKey) &&
                    values.InventoryTreatment.TreatmentKey === treatment &&
                    values.Location.LocationKey === location.LocationKey) ? 
                        values.QuantityPicked : 
                        0;
        });

        this.QuantityRemaining = ko.pureComputed(function () {
            return (quantityAvailable() || 0) + (this.InitialQuantityPicked() || 0) - (this.QuantityPicked() || 0);
        }, this);

        this.InventoryValidationMessage = ko.observable();


        function autoAddItem() {
            if (model.autoAddItem) {
                model.autoAddItem();
            }
        }

        this.IsValidInventory = ko.computed(function () {
            var lotKey = model.LotKey(),
                location = model.OriginWarehouseLocation(),
                matchingInventory,
                maxPickValue,
                packagingProduct = model.PackagingProduct(),
                treatment = model.InventoryTreatment();

            if (location == undefined || 
                    treatment == undefined || 
                    packagingProduct == undefined) {
                return false;
            }

            matchingInventory = ko.utils.arrayFirst(model.InventoryForLot(), function (item) {
                return (item.LotKey === lotKey) && 
                    ((item.PackagingProduct && 
                          item.PackagingProduct.ProductKey === packagingProduct.ProductKey) || 
                          item.PackagingProductKey === packagingProduct.ProductKey) &&
                    ((item.InventoryTreatment && 
                          item.InventoryTreatment.TreatmentKey === treatment) || 
                          item.TreatmentKey === treatment) &&
                    ((item.Location && 
                          item.Location.LocationKey === location.LocationKey) ||
                          item.WarehouseLocationKey === location.LocationKey);
            });

            if (!matchingInventory) {
                quantityAvailable(0);
                model.InventoryValidationMessage('Inventory cannot be found for this lot, packaging, treatment and location');
                console.log('Inventory cannot be found for this lot, packaging, treatment and location');

                return false;
            } else {
                model.OriginWarehouseLocationsWithInventory(model.InventoryForLot());
                model.InventoryKey(matchingInventory.InventoryKey);
                maxPickValue = (matchingInventory.Quantity || 0) + model.InitialQuantityPicked();
                quantityAvailable(matchingInventory.Quantity);

                if (model.QuantityPicked() <= maxPickValue) {
                    return true;
                }

                model.InventoryValidationMessage('Invalid picked quantity');

                return false;
            }
        });

        this.QuantityPicked.extend({
            validation: [
                {
                    validator: function() {
                        return model.IsValidInventory();
                    },
                    message: function() {
                        return "Inventory selection results in negative inventory";
                    }
                }
            ]
        });
        this.isInvalidSelection = ko.pureComputed(function () {
            return (this.QuantityPicked() &&
                !this.IsValidInventory());
        }, this);

        var validation = ko.validatedObservable({
            quantity: model.QuantityPicked,
            destination: model.DestinationWarehouseLocationValue,
            inventoryKey: model.InventoryKey,
        });

        this.isValid = function() {
            var valid = validation.isValid();
            if (!valid) { validation.errors.showAllMessages(); }
            return valid;
        };

        warehouseLocationSubscription = this.OriginWarehouseLocation.subscribe(function () {
            if (!model.LotKey() || !model.OriginWarehouseLocation()) {
                return;
            }

            var matchingInventory = ko.utils.arrayFilter(model.InventoryForLot(), function (item) {
                return item.LotKey === model.LotKey() && 
                    item.LocationKey === model.OriginWarehouseLocation.keyValue();
            });

            if (matchingInventory.length === 1) {
                model.PackagingProduct.keyValue(matchingInventory[0].PackagingProduct.ProductKey);
                model.InventoryTreatment(matchingInventory[0].InventoryTreatment.TreatmentKey);
                return;
            }
        });

        this.dispose = function () {
            model.OriginWarehouseLocation.dispose();
            warehouseLocationSubscription.dispose();
        };

        helpers.ajaxStatusHelper(this.LotKey);

        return model;

        function searchLotKey(lotKey) {
            if (getInventoryDeferred && getInventoryDeferred.state() === "pending") {
                getInventoryDeferred.reject();
            }

            // IE fix to prevent focus from going to the navigation field
            var focus = $(document.activeElement),
              index = $('.form-control').index(focus) + 1;
            $('.form-control').eq(index).focus();

            model.LotKey.indicateWorking();

            getInventoryDeferred = inventoryService.getInventoryByLot(lotKey)
                .done(function (response) {
                    if (!(response && response.InventoryItems)) {
                        throw new Error("Invalid Response");
                    }

                    model.isPopulated(true);
                    model.InventoryForLot(response.InventoryItems || []);

                    if (!response.InventoryItems.length) {
                        showUserMessage("No Inventory Available", { description: "There is no inventory available for the lot <strong>" + lotKey + "</strong>." });
                        model.LotKey.indicateFailure();
                        return;
                    }

                    model.LotKey.indicateSuccess();

                    var item = response.InventoryItems[0];
                    model.InventoryProductName(item.Product.ProductName);
                    model.PackagingProduct.keyValue(item.PackagingProduct.ProductKey);
                    model.InventoryTreatment(item.InventoryTreatment.TreatmentKey);
                    model.OriginWarehouseLocation(item.Location);
                    model.OriginWarehouseLocation.keyValue(item.Location.LocationKey);
                    autoAddItem();

                    if (model.OriginWarehouseLocationsWithInventory().length > 1) {
                        model.originWarehouseLocationHasFocus(true);
                    } else if (model.PackagingProductOptions().length > 1) {
                        model.packagingProductOptionsHasFocus(true);
                    } else model.treatmentOptionsHasFocus(true);
                })
                .fail(function (xhr, status, message) {
                    model.LotKey.indicateFailure();
                    // not found or other server error
                    if (xhr.status === 404) {
                        showUserMessage("No inventory found for lot");
                    } else {
                        showUserMessage("Failed to get inventory", { description: message });                    
                    }
                });
        }
    }
});

module.exports = {
    viewModel: InventoryPickOrderViewModel,
    template: templateMarkup
};
