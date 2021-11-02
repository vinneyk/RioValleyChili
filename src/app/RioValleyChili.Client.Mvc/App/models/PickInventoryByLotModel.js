var inventoryService = require('services/inventoryService'),
    helpers = require('App/helpers/koHelpers');

require('App/scripts/ko.extenders.lotKey');

function PickInventoryByLot(input, config) {
    if (!config || !config.warehouseLocationOptions) throw new Error('Missing configuration setting: warehouseLocationOptions');

    if (!(this instanceof PickInventoryByLot)) return new PickInventoryByLot(input, config);

    // Init
    var self = this,
        data = input || {},
        quantityAvailable = ko.observable(0),
        getInventoryDeferred;

    // Data
    this.isPopulated = ko.observable(false);
    this.isDefaultSetterParam = !!(config.setDefaultInventorySelectionsForLot);
    this.isDefaultGetterParam = !!(config.getDefaultInventorySelectionsForLot);

    this.QuantityPicked = ko.numericObservable(data.QuantityPicked);
    this.LotKey = ko.observable(data.LotKey).extend({ lotKey: { changedCallback: searchLotKey } });
    this.InventoryKey = ko.observable().extend({ 
        required: { 
            onlyIf: function () { 
                return self.LotKey() != undefined; 
            }
        }
    });

    this.OriginWarehouseLocation = ko.observable(data.Location);
    this.LocationOptions = ko.observableArray([]);

    this.originWarehouseLocationHasFocus = ko.observable();
    this.packagingProductOptionsHasFocus = ko.observable();
    this.treatmentOptionsHasFocus = ko.observable();

    this.InventoryProductName = ko.observable(data.Product && data.Product.ProductCodeAndName);

    if (data.Product) {
        data.PackagingProductKey = data.PackagingProduct.ProductKey;
        data.TreatmentKey = data.InventoryTreatment.TreatmentKey;
        data.WarehouseLocationKey = data.Location.LocationKey;
    }

    this.InventoryForLot = ko.observableArray(data.Product ? [data] : []);

    this.PackagingProduct = ko.observable(data.PackagingProduct);
    this.PackagingProductOptions = ko.observableArray([]);
        
    this.InventoryTreatment = ko.observable(data.InventoryTreatment && data.InventoryTreatment.TreatmentKey);

    this.InventoryTreatmentOptions = ko.observableArray([]);

    this.hasLotError = ko.pureComputed(function() {
      var ajaxCheck = self.LotKey.hasOwnProperty('ajaxFailure') && self.LotKey.ajaxFailure();

      return ajaxCheck;
    });
    this.hasError = ko.pureComputed(function() {
      return ko.unwrap(self.QuantityPicked) && !ko.unwrap(self.IsValidInventory);
    });

    var lastInventory, lastLotNumber, lastLocationValue, lastPackagingValue;

    ko.computed(function () {
        var selectedLocation = self.OriginWarehouseLocation();
        var selectedPackaging = self.PackagingProduct();

        function isPackagingValidForSelection(item) {
            return selectedLocation != undefined && selectedLocation === item.Location.LocationKey;
        }
        function isTreatmentValidForSelection(item) {
            return selectedPackaging != undefined && isPackagingValidForSelection(item)
                && selectedPackaging.ProductKey === item.PackagingProduct.ProductKey;
        }

        var availableInventory = self.InventoryForLot() || [],
            validLocationOptions = [],
            validLocationOptionsCache = {},
            validPackagingOptions = [],
            validPackagingOptionsCache = {},
            validTreatmentOptions = [],
            validTreatmentOptionsCache = {},
            rebuildLocationOptions = hasInventoryChanged() || hasLotNumberChanged(),
            rebuildPackagingOptions = originWarehouseLocationHasChanged() || rebuildLocationOptions,
            rebuildTreatmentOptions = packagingValueHasChanged() || rebuildPackagingOptions;
            
        function hasInventoryChanged() {
            return ((lastInventory && lastInventory.length) || 0) !== availableInventory.length;
        }

        var lotNumber = self.LotKey.formattedLot();
        if (!(rebuildLocationOptions || rebuildPackagingOptions || rebuildTreatmentOptions)) { return; }

        if (lotNumber != undefined) {
            buildCascadeOptions();
        }

        if (rebuildLocationOptions) {
            self.LocationOptions(validLocationOptions);
            self.OriginWarehouseLocation(validLocationOptions.length === 1 ? validLocationOptions[0] : null);
            rebuildLocationOptions = false;
        }

        if (rebuildPackagingOptions) {
            self.PackagingProductOptions(validPackagingOptions);

            var packaging = validPackagingOptions.length === 1 ? validPackagingOptions[0] : null;
            self.PackagingProduct(packaging);

            // For some unknown reason, setting PackagingProduct is not triggering the computed to be reevaluated. 
            // Instead, we're manually resetting control variables and calling the `buildCascadeOptions()` function.
            if (packaging) {
                rebuildPackagingOptions = false;
                selectedPackaging = packaging;
                buildCascadeOptions();
            }
        }

        if (rebuildTreatmentOptions) {
            self.InventoryTreatmentOptions(validTreatmentOptions);
            self.InventoryTreatment(validTreatmentOptions.length === 1 ? validTreatmentOptions[0].TreatmentKey : null);
            rebuildTreatmentOptions = false;
        }

        lastLotNumber = lotNumber;
        lastInventory = lastLotNumber == undefined ? [] : availableInventory;
        lastLocationValue = self.OriginWarehouseLocation();
        lastPackagingValue = self.PackagingProduct();

        function buildCascadeOptions() {
            ko.utils.arrayForEach(availableInventory, function (item) {
                if (rebuildLocationOptions) {
                    if (item.Location && validLocationOptionsCache[item.Location.LocationKey] == undefined) {
                        validLocationOptionsCache[item.Location.LocationKey] = item.Location;
                        validLocationOptions.push(item.Location);
                    }
                }

                if (rebuildPackagingOptions) {
                    if (item.PackagingProduct && isPackagingValidForSelection(item) && validPackagingOptionsCache[item.PackagingProduct.ProductKey] == undefined) {
                        validPackagingOptionsCache[item.PackagingProduct.ProductKey] = item.PackagingProduct;
                        validPackagingOptions.push(item.PackagingProduct);
                    }
                }

                if (rebuildTreatmentOptions) {
                    if (item.InventoryTreatment && isTreatmentValidForSelection(item) && validTreatmentOptionsCache[item.InventoryTreatment.TreatmentKey] == undefined) {
                        validTreatmentOptionsCache[item.InventoryTreatment.TreatmentKey] = item.InventoryTreatment;
                        validTreatmentOptions.push(item.InventoryTreatment);
                    }
                }
            });
        }

        function originWarehouseLocationHasChanged() {
            return lastLocationValue !== self.OriginWarehouseLocation();
        }

        function packagingValueHasChanged() {
            return lastPackagingValue !== self.PackagingProduct();
        }

        function hasLotNumberChanged() {
            return lastLotNumber !== self.LotKey.formattedLot();
        }
    });
        
    this.WeightPicked = ko.pureComputed(function () {
        var packaging = this.PackagingProduct();
        return (packaging ? packaging.Weight : 0) * (this.QuantityPicked() || 0);
    }, this);

    this.WeightPicked.formatted = ko.pureComputed(function () {
        return this.WeightPicked().toLocaleString();
    }, this);

    this.InitialQuantityPicked = ko.computed(function () {
        var lotKey = self.LotKey(),
            packagingProduct = self.PackagingProduct(),
            treatment = self.InventoryTreatment(),
            location = self.OriginWarehouseLocation();

        if (lotKey == undefined || packagingProduct == undefined || treatment == undefined || location == undefined) return 0;

        return (data.LotKey === lotKey &&
                data.PackagingProduct.ProductKey === (typeof packagingProduct === "string" ? packagingProduct : packagingProduct.ProductKey) &&
                data.InventoryTreatment.TreatmentKey === treatment &&
                data.Location.LocationKey === location.LocationKey) ? 
                    data.QuantityPicked : 
                    0;
    });

    this.QuantityRemaining = ko.pureComputed(function () {
        return (quantityAvailable() || 0) + (this.InitialQuantityPicked() || 0) - (this.QuantityPicked() || 0);
    }, this);

    this.InventoryValidationMessage = ko.observable();


    function autoAddItem() {
        if (self.autoAddItem) {
            self.autoAddItem();
        }
    }

    this.IsValidInventory = ko.computed(function () {
        var lotKey = self.LotKey(),
            locationKey = self.OriginWarehouseLocation(),
            packagingProduct = self.PackagingProduct(),
            treatment = self.InventoryTreatment(),
            quantityPicked = self.QuantityPicked();

        if (anyUndefined([locationKey, treatment, packagingProduct])) {
            return false;
        }

        var matchingInventory = ko.utils.arrayFirst(self.InventoryForLot(), function (item) {
            return (item.LotKey === lotKey) && 
            ((item.PackagingProduct && 
                    item.PackagingProduct.ProductKey === packagingProduct.ProductKey) || 
                item.PackagingProductKey === packagingProduct.ProductKey) &&
            ((item.InventoryTreatment && 
                    item.InventoryTreatment.TreatmentKey === treatment) || 
                item.TreatmentKey === treatment) &&
            ((item.Location && 
                    item.Location.LocationKey === locationKey) ||
                item.WarehouseLocationKey === locationKey);
        });

        if (!matchingInventory) {
            quantityAvailable(0);
            self.InventoryValidationMessage('Inventory cannot be found for this lot, packaging, treatment and location');
            return false;
        }

        self.InventoryKey(matchingInventory.InventoryKey);
        var maxPickValue = (matchingInventory.Quantity || 0) + self.InitialQuantityPicked();
        quantityAvailable(matchingInventory.Quantity);

        if (quantityPicked <= maxPickValue) {
            return true;
        }

        self.InventoryValidationMessage('Invalid picked quantity');

        return false;
    });

    this.QuantityPicked.extend({
        validation: [
            {
                validator: function() {
                    return self.IsValidInventory();
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
        quantity: self.QuantityPicked,
        inventoryKey: self.InventoryKey,
    });
    this.isValid = function() {
        var valid = validation.isValid();
        if (!valid) { validation.errors.showAllMessages(); }
        return valid;
    };

    this.dispose = function () {
        self.OriginWarehouseLocation.dispose();
        if (self.defaultSetter) {
            self.defaultSetter.dispose();
        }
    };

    helpers.ajaxStatusHelper(this.LotKey);

    function searchLotKey(rawValue, lotKey) {
        if (getInventoryDeferred && getInventoryDeferred.state() === "pending") {
            getInventoryDeferred.reject();
        }

        if (lotKey == undefined) {
            self.InventoryForLot([]);
            return;
        }

        self.LotKey.indicateWorking();
        self.isInit = ko.observable(false);
        self.cachedLotKey = ko.observable('');

        getInventoryDeferred = inventoryService.getInventoryByLot(lotKey)
            .done(function (response) {
                if (!(response && response.InventoryItems)) {
                    throw new Error("Invalid Response");
                }
                self.isInit(false);

                self.isPopulated(true);
                var inventoryResult = response.InventoryItems || [];
                self.InventoryForLot(inventoryResult);

                if (!response.InventoryItems.length) {
                    showUserMessage("No Inventory Available", { description: "There is no inventory available for the lot <strong>" + lotKey + "</strong>." });
                    self.LotKey.indicateFailure();
                    return;
                }

                self.LotKey.indicateSuccess();

                autoAddItem();

                if (inventoryResult.length) {
                    self.InventoryProductName(inventoryResult[0].Product.ProductCodeAndName);
                }

                if (self.LocationOptions().length > 1) {
                    self.originWarehouseLocationHasFocus(true);
                } else if (self.PackagingProductOptions().length > 1) {
                    self.packagingProductOptionsHasFocus(true);
                } else {
                    self.treatmentOptionsHasFocus(true);
                }

            })
            .fail(function (xhr, status, message) {
                self.LotKey.indicateFailure();
                // not found or other server error
                if (xhr.status === 404) {
                    showUserMessage("No inventory found for lot");
                } else {
                    showUserMessage("Failed to get inventory", { description: message });                    
                }
            })
            .always(function() {
                if (self.isDefaultGetterParam) {
                    var defaults = config.getDefaultInventorySelectionsForLot(lotKey);

                    //self.PackagingProduct(defaults.defaultPackagingKey);
                    self.OriginWarehouseLocation(defaults.defaultLocationKey);
                    self.InventoryTreatment(defaults.defaultTreatmentKey);
                }

                if (self.isDefaultSetterParam) {
                    self.cachedLotKey(lotKey);

                    if (self.defaultSetter) {
                        self.isInit(false);
                    }
                    self.defaultSetter = buildDefaultUpdateService();
                }

                self.isInit(true);

                function buildDefaultUpdateService() {
                    var saveLotSelections = ko.computed({
                        read: function() {
                            var init = self.isInit;
                            if (!init()) {
                                return;
                            }

                            var lot = self.LotKey.formattedLot(),
                                location = self.OriginWarehouseLocation(),
                                packaging = self.PackagingProduct(),
                                treatment = self.InventoryTreatment();

                            if (lot && lot.length >= 12) {
                                config.setDefaultInventorySelectionsForLot(lot, {
                                    defaultLocationKey: location,
                                    defaultPackagingKey: packaging,
                                    defaultTreatmentKey: treatment
                                });
                            }
                        },
                        disposeWhen: function() {
                            var lot = self.LotKey.formattedLot(),
                                cache = self.cachedLotKey();

                            if (cache !== '' && lot !== cache) {
                                self.isInit(false);
                                return true;
                            }
                        }
                    });

                    return saveLotSelections;
                }

            });
    }

    return this;

    function anyUndefined(items) {
        for (var i = 0; i < items.length; i++) {
            if (items[i] == undefined) {
                return true;
            }
        }
        return false;
    }
}

module.exports = PickInventoryByLot
