'use strict';

function PickedInventory(data) {
    var initialPicked = parseInt(ko.utils.unwrapObservable(data.QuantityPicked)) || 0,
        initialQtyAvailable = (parseInt(ko.utils.unwrapObservable(data.Quantity)) || 0) + initialPicked,
        destination = data.DestinationWarehouseLocation || {};
    
    
    var model = {
        pickedInventoryItemKey: data.PickedInventoryItemKey,
        inventoryKey: data.InventoryKey,
        lotNumber: data.LotKey,
        quantityPicked: ko.numericObservable(initialPicked, 0).extend({ min: 0, max: initialQtyAvailable }),
        productKey: data.InventoryProduct.ProductKey,
        productName: data.InventoryProduct.ProductName,
        productType: ko.observable(data.InventoryProduct.ProductType).extend({ productType: true }),
        packagingCapacity: data.PackagingProduct.Weight,
        packagingName: data.PackagingProduct.ProductName,
        warehouseName: data.WarehouseLocation.WarehouseName,
        warehouseKey: data.WarehouseLocation.WarehouseKey,
        warehouseLocationName: data.WarehouseLocation.WarehouseLocationName,
        warehouseLocationKey: data.WarehouseLocation.WarehouseLocationKey,
        destinationWarehouseKey: ko.observable(destination.WarehouseKey),
        destinationWarehouseName: ko.observable(destination.WarehouseName),
        destinationLocationKey: ko.observable(destination.WarehouseLocationKey),
        destinationLocationName: ko.observable(destination.WarehouseLocationName),
    };
    model.isPicked = ko.computed(function () {
        return this.quantityPicked() > 0;
    }, model);
    model.weightPicked = ko.computed(function () {
        return (ko.utils.unwrapObservable(this.packagingCapacity) || 0) * (this.quantityPicked() || 0);
    }, model);
    model.quantityAvailable = ko.computed(function () {
        return initialQtyAvailable - (this.quantityPicked() || 0);
    }, model);
    model.weightAvailable = ko.computed(function () {
        return this.quantityAvailable() * this.packagingCapacity;
    }, model);

    var errors = ko.validation.group(model);
    model.isValid = ko.computed(function() {
        return errors().length === 0;
    }, errors);


    return model;
}

var PickedInventoryViewModel = (function (ajaxStatusHelper) {

    return {
        init: init
    };
    
    function init(options) {
        if (!options.data) {
            options.target.PickInventoryViewModel = null;
            return null;
        }
        
        var data = options.data;
        if (data.Key == undefined) throw new Error("Invalid input data. Expected property \"Key\" to reference the key value of the parent context.");
        if (data.PickedInventoryItems == undefined) throw new Error("Invalid input data. Expected property \"pickedInventoryItems\".");
        if (!options.target) throw new Error("Target option is required.");

        var pickOrderType = ko.observable(options.mode).extend({ inventoryOrderTypes: null });
        if (!pickOrderType()) throw new Error("Valid pickOrderType is required.");
        if (options.isLocked == undefined) throw new Error("Please indicate whether the inventory order is locked.");

        var viewModel = {};
        viewModel.pickedInventoryItems = ko.observableArray([]);
        viewModel.productTypeFilter = ko.observable().extend({ productType: true });
        var isLocked = ko.observable(options.isLocked);

        // init
        initPickedItems();
        var isValid = ko.computed(function () {
            return ko.utils.arrayFirst(this(), function (item) {
                return !item.isValid();
            }) === null;
        }, viewModel.pickedInventoryItems);
        
        // computed properties
        viewModel.pickedInventoryTypes = ko.computed(function () {
            var pickedTypes = ko.utils.arrayMap(this.pickedInventoryItems(), function (item) {
                return ko.utils.unwrapObservable(item.productType);
            });
            var distinctTypes = ko.utils.arrayGetDistinctValues(pickedTypes);
            
            return distinctTypes;
        }, viewModel);
        viewModel.pickedInventoryItemsForType = ko.computed(function () {
            var filtered = ko.utils.arrayFilter(this.pickedInventoryItems(), function (item) {
                return (ko.utils.unwrapObservable(item.productType) === parseInt(viewModel.productTypeFilter()));
            });
            return filtered;
        }, viewModel);
        viewModel.countOfItemsPicked = ko.computed(function() {
            var countOfItems = 0;
            ko.utils.arrayForEach(this.pickedInventoryItems(), function(item) {
                countOfItems += item.quantityPicked();
            });
            return countOfItems;
        }, viewModel),
        viewModel.sumOfWeightPicked = ko.computed(function() {
            var sumOfWeight = 0;
            ko.utils.arrayForEach(this.pickedInventoryItems(), function (item) {
                sumOfWeight += item.weightPicked();
            });
            return sumOfWeight;
        }, viewModel),
        viewModel.sumOfWeightPickedDisplay = ko.computed(function() {
            return this.sumOfWeightPicked().addFormatting(0) + " lbs.";
        }, viewModel);
        
        EsmHelper(viewModel, {
            customMappings: {
                pickedInventoryItems: function(data) {
                    return ko.utils.arrayMap(data, function(item) { return new PickedInventory(getDataFromViewModel(item)); });
                },
            },
            revertChangesCallback: function() {
                viewModel.saveCommand.clearStatus();
                viewModel.saveCommand.clearResults();
            },
            canEdit: ko.computed(function() {
                return !isLocked();
            }),
            ignore: ['productTypeFilter', 'isLocked'],
        });

        // methods
        viewModel.savePickedItems = function (opts) {
            savePickedItems(data.Key, pickOrderType.displayValue(), viewModel.toDto(), opts);
        };
        viewModel.toDto = toDto;

        // commands
        viewModel.saveCommand = ko.composableCommand({ 
            execute: function (complete) {
                viewModel.saveCommand.indicateWorking();
                viewModel.savePickedItems({
                    successCallback: success,
                    errorCallback: error,
                    completeCallback: complete,
                });
                
                function success() {
                    viewModel.saveEditsCommand.execute();
                    viewModel.saveCommand.pushSuccess("Picked Inventory Updated Successfully");
                    viewModel.saveCommand.indicateSuccess();
                }
                function error(xhr) {
                    showUserMessage("Failed to Save Picked Inventory.", { description: xhr.responseText, mode: 'error' });
                    viewModel.saveCommand.pushError("Picked Inventory failed to save. " + xhr.responseText);
                    viewModel.saveCommand.indicateFailure();
                }
            },
            canExecute: function(isExecuting) {
                return !isExecuting && !isLocked() && isValid();
            },
            shouldExecute: function () { return viewModel.hasChanges(); },
        });
        viewModel.addPickedItemCommand = ko.command({
            execute: function(item) {
                mergePickedItems.call(viewModel, item);
            },
            canExecute: function() {
                return !isLocked();
            }
        });
        viewModel.removePickedItemCommand = ko.command({
            execute: function (item) {
                var index = ko.utils.arrayIndexOf(viewModel.pickedInventoryItems(), item);
                if (index >= 0) viewModel.pickedInventoryItems.splice(index, 1);
            },
            canExecute: function() { return !isLocked(); }
        });

        ajaxStatusHelper.init(viewModel.saveCommand);

        options.target.PickInventoryViewModel = viewModel;

        return viewModel;
        
        function initPickedItems() {
            viewModel.pickedInventoryItems(ko.utils.arrayMap(data.PickedInventoryItems, function(item) {
                return new PickedInventory(item);
            }));
        }
    };

    //#region private functions

    function getDataFromViewModel(item) {
        return ko.toJS({
            PickedInventoryItemKey: item.pickedInventoryItemKey,
            InventoryKey: item.inventoryKey,
            LotKey: item.lotNumber,
            QuantityPicked: item.quantityPicked,
            InventoryProduct: {
                ProductKey: item.productKey,
                ProductName: item.productName,
                ProductType: item.productType
            },
            PackagingProduct: {
                ProductKey: item.packagingKey,
                ProductName: item.packagingName,
                Weight: item.packagingCapacity,
            },
            WarehouseLocation: {
                WarehouseLocationKey: item.warehouseLocationKey,
                WarehouseLocationName: item.warehouseLocationName,
                WarehouseKey: item.warehouseKey,
                WarehouseName: item.warehouseName,
            },
            DestinationWarehouseLocation: {
                WarehouseLocationKey: item.destinationLocationKey,
                WarehouseLocationName: item.destinationLocationName,
                WarehouseKey: item.destinationWarehouseKey,
                WarehouseName: item.destinationWarehouseName,
            },
            Quantity: item.quantityAvailable
        });
    }
    function mergePickedItems(newItem) {
        var existing = getPickedItemByKey.call(this, ko.utils.unwrapObservable(newItem.inventoryKey), ko.utils.unwrapObservable(newItem.destinationLocationKey));
        if (existing) {
            var newQty = parseInt(existing.quantityPicked()) + parseInt(newItem.quantityPicked());
            existing.quantityPicked(newQty);
            return;
        }

        var mapped = new PickedInventory(getDataFromViewModel(newItem));
        this.pickedInventoryItems.push(mapped);
        this.pickedInventoryItems.notifySubscribers(mapped, 'pickedItemAdded');
    }
    function getPickedItemByKey(inventoryKey, destinationLocationKey) {
        if (!inventoryKey) return null;
        
        var vm = this;
        return ko.utils.arrayFirst(vm.pickedInventoryItems(), function (pickedItem) {
            return inventoryKey === ko.utils.unwrapObservable(pickedItem.inventoryKey)
                && !destinationLocationKey || ko.utils.unwrapObservable(pickedItem.destinationLocationKey) === destinationLocationKey;
        });
    }
    function savePickedItems(orderKey, pickOrderType, dto, options) {
        $.ajax({
            url: "/api/" + pickOrderType + "/pickedInventory/" + orderKey,
            type: 'PUT',
            contentType: 'application/json',
            data: ko.toJSON(dto),
            success: options.successCallback,
            error: options.errorCallback,
            complete: options.completeCallback,
        });
    }
    function toDto() {
        return {
            pickedInventoryItems: ko.utils.arrayMap(
                ko.utils.arrayFilter(this.pickedInventoryItems(), isPicked),
                pickedInventoryItemDtoMapping)
        };
        
        function isPicked(item) {
            return item.quantityPicked() > 0;
        }
        
        function pickedInventoryItemDtoMapping(pickedItem) {
            return {
                InventoryKey: pickedItem.inventoryKey,
                Quantity: pickedItem.quantityPicked,
                DestinationWarehouseLocationKey: ko.utils.unwrapObservable(pickedItem.destinationLocationKey),
            };
        }
    }

    //#endregion

}(ajaxStatusHelper));

var PickInventoryHelper = (function (pickInventoryViewModel) {
    // This utility is used to wire up related operations between inventoryViewModel and 
    // pickedInventoryViewModel in the context of a given parent view model object. 

    return {
        init: init,
    };

    function init(target, options) {
        target.mapInventoryItemsForPicking = mapInventoryItemsForPicking;
        
        initInventoryViewModel();
        initPickInventoryViewModel();
        wirePickInventoryOperations.call(this, target);
        
        function initInventoryViewModel() {
            inventoryViewModel.init({
                baseUrl: options.inventoryUrl,
                initialInventoryItems: options.pickedInventoryItems,
            }, target);
        }

        function initPickInventoryViewModel() {
            pickInventoryViewModel.init({
                pickedInventoryKey: options.pickedInventoryKey,
                pickedInventoryItems: target.mapInventoryItemsForPicking(options.pickedInventoryItems || []),
            }, {
                mode: options.inventoryOrderMode,
                isLocked: options.isLocked,
            }, target);

            setupCalculatedAverages();
            
            function setupCalculatedAverages() {
                target.attributeAverages = ko.observable();
                target.getAverageForAttribute = getCalculationForAttribute;
                target.pickedInventoryItems.subscribe(getCalculatedAveragesForPickedItems, target);
                getCalculatedAveragesForPickedItems(target.pickedInventoryItems());
                
                function getCalculatedAveragesForPickedItems(pickedItems) {
                    if (pickedItems.length < 1) {
                        target.attributeAverages(null);
                        return;
                    }
                    var pickInventoryData = ko.toJSON(ko.utils.arrayMap(pickedItems, function(item) {
                        return {
                            InventoryKey: item.inventoryKey,
                            Quantity: item.quantityPicked(),
                        };
                    }));

                    var url = "/api/inventory/pickedCalculations";
                    $.ajax({
                        url: url,
                        cache: false,
                        contentType: 'application/json',
                        success: function (data) {
                            var calculations = {};
                            for (var prop in data) {
                                calculations[prop] = parseFloat(data[prop]).toFixed(3);
                            }
                            target.attributeAverages(calculations);
                        },
                        type: 'POST',
                        data: pickInventoryData,
                    }).fail(function() {
                        showUserMessage("Unable to calculate weighted averages.", {
                            description: "An error occurred while attempting to calculate weighted averages for the picked items.",
                            mode: 'error'
                        });
                    });
                }
                function getCalculationForAttribute(attributeName) {
                    var averages = this.attributeAverages();
                    if (averages) {
                        var value = averages[attributeName];
                        if (value !== undefined) return value;
                    }
                    return "-";
                }
            }
        }
    }
    
    function wirePickInventoryOperations(target) {  // target = pickedInventoryViewModel
        var initialPickedData = mapPickedInventoryItemsWithDetails(target.pickedInventoryItems());
        target.pickedInventoryDetails = ko.observableArray(initialPickedData);
        target.pickedInventoryDetailsForFilter = ko.computed(function () {
            //todo: refactor into inventory view model
            return ko.utils.arrayFilter(target.pickedInventoryDetails(), function (pickedItem) {
                return inventoryTypeFilter.call(target, pickedItem)
                    && lotTypeFilter.call(target, pickedItem)
                    && warehouseFilter.call(target, pickedItem);
            });
            
            function inventoryTypeFilter(item) {
                return !this.inventoryTypeFilter() || this.inventoryTypeFilter() === item.ProductType;
            }
            function lotTypeFilter(item) {
                return !this.lotTypeFilter() || this.lotTypeFilter() === item.inventoryType;
            }
            function warehouseFilter(item) {
                return !this.warehouseFilter() || this.warehouseFilter() === item.WarehouseKey;
            }
        }, target);
        target.inventoryItems.subscribe(function () {
            // Notifications to pickedInventoryDetails are deferred so that the
            // inventory items are not displayed in duplicate (once in 
            // pickableInventoryItems and again in pickedInventoryItems). 
            refreshPickedInventoryDetailsSubscriptions.call(target);
        }, null, 'beforeChange');
        target.pickedInventoryItems.subscribe(function (pickedItem) {
            var mapped = mapPickedInventoryWithDetails.call(target, pickedItem);
            target.pickedInventoryDetails().push(mapped);
        }, null, 'pickedItemAdded');
        
        setupPickableInventoryItems();

        target.savePickingCommand = ko.composableCommand({
            children: [target.saveCommand],
            execute: function(complete) {
                refreshPickedInventoryDetailsSubscriptions.call(target);
                complete();
            },
            moduleDependency: ko.composableCommand.moduleDependency.allModulesRequired,
            log_name: 'savePickingCommand'
        });

        function mapPickedInventoryItemsWithDetails(pickedInventoryItems) {
            return ko.utils.arrayMap(pickedInventoryItems, mapPickedInventoryWithDetails.bind(target));
        }
        function mapPickedInventoryWithDetails(pickedItem) {
            var inventoryDetail = ko.utils.arrayFirst(this.inventoryItems(), findByKeyPredicate, pickedItem);
            if (!inventoryDetail) return null;

            if (!ko.isObservable(inventoryDetail.quantityPicked)) {
                inventoryDetail.quantityPicked = pickedItem.quantityPicked;
                mapInventoryItemsForPicking([inventoryDetail]);
            }
            
            return inventoryDetail;

            function findByKeyPredicate(item) {
                return ko.utils.unwrapObservable(item.inventoryKey) === ko.utils.unwrapObservable(this.inventoryKey);
            }
        }
        function setupPickableInventoryItems() {
            target.pickableInventoryItems = ko.computed(function () {
                var pc = new PickedItemsCache(target.pickedInventoryDetailsForFilter() || []);
                var unpickedItems = ko.utils.arrayFilter(target.inventoryItems(), function (item) {
                    return !pc.findByKey(item.InventoryKey);
                });
                return target.mapInventoryItemsForPicking(unpickedItems);
            });
        }
    }

    function mapInventoryItemsForPicking(items) {
        var target = this;

        return ko.utils.arrayMap(items, function (item) { return mapInventoryItemToPickItem.call(item); });
        
        function mapInventoryItemToPickItem() {
            if (!itemIsAlreadyMapped.call(this)) mapNewItem.call(this);
            
            function itemIsAlreadyMapped() {
                return this.quantityPicked && ko.isWriteableObservable(this.quantityPicked);
            }
            function mapNewItem() {
                var initialQuantity = ko.utils.unwrapObservable(this.Quantity);
                var initialQtyPicked = this.QuantityPicked || 0;
                var quantityOnHand = initialQuantity + initialQtyPicked;
                
                this.inventoryKey = this.InventoryKey;
                this.productType = this.ProductType;
                this.productName = this.ProductName;
                this.productKey = this.ProductKey;
                this.lotNumber = this.LotKey;
                this.packagingKey = this.PackagingProductKey;
                this.packagingName = this.PackagingProductName;
                this.packagingCapacity = this.PackagingCapacity;
                this.quantityAvailable = this.Quantity;
                this.warehouseKey = this.WarehouseKey;
                this.warehouseName = this.WarehouseName;
                this.warehouseLocationKey = this.WarehouseLocationKey;
                this.warehouseLocationName = this.WarehouseLocationName;
                this.destinationWarehouseKey = this.DestinationWarehouseLocation ? this.DestinationWarehouseLocation.WarehouseKey : null;
                this.destinationWarehouseName = this.DestinationWarehouseLocation ? this.DestinationWarehouseLocation.WarehouseName : null;
                this.destinationWarehouseLocationKey = this.DestinationWarehouseLocation ? this.DestinationWarehouseLocation.WarehouseLocationKey : null;
                this.destinationWarehouseLocationName = this.DestinationWarehouseLocation ? this.DestinationWarehouseLocation.WarehouseLocationName : null;
                this.quantityPicked = ko.numericObservable(initialQtyPicked)
                    .extend({ min: 0, max: ko.utils.unwrapObservable(quantityOnHand) });
                this.Quantity = ko.computed(function() { return quantityOnHand - this.quantityPicked(); }, this);
                
                this.quantityPickedSubscription = this.quantityPicked.subscribe(function () {
                    pickInventoryItem(this);
                }, this);
            }

            return this;
        }
        
        function pickInventoryItem(item) {
            if (item.quantityPicked() > 0 && target.addPickedItemCommand) {
                target.addPickedItemCommand.execute(item);
            }
        }
    }

    function refreshPickedInventoryDetailsSubscriptions() {
        var target = this;
        removeItemsWithZeroQuantityPicked();
        this.pickedInventoryDetails.notifySubscribers(target.pickedInventoryDetails);

        function removeItemsWithZeroQuantityPicked() {
            var zeroQuantityPickedItems = ko.utils.arrayFilter(
                target.pickedInventoryDetails(),
                function (pick) { return pick.quantityPicked() < 1; });

            ko.utils.arrayForEach(zeroQuantityPickedItems, function (zero) {
                ko.utils.arrayRemoveItem(target.pickedInventoryDetails(), zero);
            });
        }
    }
    
    function PickedItemsCache(pickedItems) {
        var pickedCache = {};
        ko.utils.arrayForEach(pickedItems, function (i) {
            pickedCache[i.inventoryKey] = i.quantityPicked();
        });

        return {
            findByKey: findByKey,
            remove: remove,
            pop: popKey,
            cache: pickedCache,
        };

        function findByKey(key) {
            if (!isNaN(pickedCache[key])) {
                return {
                    key: key,
                    quantity: pickedCache[key]
                };
            }
        }
        function remove(key) {
            delete pickedCache[key];
        }

        function popKey(key) {
            var val = findByKey(key);
            if (val) remove(key);
            return val;
        }
    }
    
}(PickedInventoryViewModel));