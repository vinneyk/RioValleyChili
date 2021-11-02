var InventoryPickOrderViewModel = (function (ko, ajaxStatusHelper, esmHelper, undefined) {

    var modes = {
        display: 'readonly',
        editing: 'editable'
    };

    var self = {
        modes: modes,
        productOptions: ko.observableArray([]),
        packagingProductOptions: ko.observableArray([]),
    };

    rvc.utils.enableProductSelectionByProductType.call(self);
    
    // constructor
    self.init = function (options) {
        if (!options.target) throw new Error("Target option is required.");
        var target = options.target;

        if (options.data) {

            var pickOrderType = ko.observable(options.mode).extend({ inventoryOrderTypes: null });
            if (pickOrderType() == undefined) throw new Error("Valid pickOrderType is required.");

            if (options.allowableLotTypes && options.allowableLotTypes.length > 0)
                self.allowableLotTypes(options.allowableLotTypes);

            var data = options.data || {};

            var viewModel = {
                PickOrderItems: ko.observableArray(ko.utils.arrayMap(data.PickOrderItems, mapPickOrderItem)),
                newItems: ko.observableArray([]),
                errors: ko.observableArray([]),

                packagingProductOptions: self.packagingProductOptions,
                productTypeOptions: self.productTypeOptions,

                // functions
                toDto: getPickOrderDto,
            };
            viewModel.clearNewItems = clearNewItems;

            var isLocked = data.IsLocked || false;

            // computed
            viewModel.allItems = ko.computed(function() {
                return viewModel.PickOrderItems().concat(viewModel.newItems());
            });
            viewModel.itemsToAdd = ko.computed(function() {
                return ko.utils.arrayFilter(this.newItems(), function(item) {
                    return item.empty() === false;
                });
            }, viewModel);
            viewModel.enablePicking = ko.computed(function() {
                return !isLocked;
            });
            viewModel.TotalQuantityPicked = ko.computed(function() {
                var quantity = 0;
                ko.utils.arrayForEach(viewModel.allItems(), function(item) {
                    quantity += ko.utils.unwrapObservable(item.Quantity) || 0;
                });
                return quantity;
            });
            viewModel.TotalWeightPicked = ko.computed(function() {
                var weight = 0;
                ko.utils.arrayForEach(viewModel.allItems(), function(item) {
                    weight += ko.utils.unwrapObservable(item.TotalWeight) || 0;
                });
                return weight + " lbs";
            });
            viewModel.HasItems = ko.computed(function() {
                return viewModel.allItems().length > 0;
            });
            viewModel.hasNewItems = ko.computed(function() {
                return viewModel.itemsToAdd().length > 0;
            });
            viewModel.isValid = ko.computed(function() {
                var items = this.allItems();
                if ((items.length || 0) === 0) return false;

                var firstInvalid = ko.utils.arrayFirst(items, function(item) {
                    return (item.Product() && parseInt(item.Quantity()) > 0) === false;
                }, this);
                return firstInvalid === null;
            }, viewModel);


            esmHelper(viewModel, {
                ignore: ['productOptions', 'packagingProductOptions', 'newItems', 'allItems', 'itemsToAdd', 'PackagingProuctOptions'],
                name: 'inventoryPickOrder',
                customMappings: cacheMapping,
                hasUntrackedChanges: function() { return viewModel.hasNewItems(); },
                commitUntrackedChanges: function() { saveNewItems.call(viewModel); },
                revertUntrackedChanges: function() { viewModel.clearNewItems(); },
                enableLogging: true,
            });

            // commands
            viewModel.saveCommand = ko.composableCommand({
                shouldExecute: function() { return viewModel.hasChanges(); },
                execute: function(complete) {
                    viewModel.saveCommand.indicateWorking();
                    savePickOrder(data.InventoryPickKey, pickOrderType, viewModel.toDto(), {
                        successCallback: successCallback,
                        errorCallback: errorCallback,
                        completeCallback: complete,
                    });

                    function successCallback() {
                        viewModel.saveCommand.indicateSuccess();
                        viewModel.saveCommand.pushSuccess("Inventory Pick Order Saved Successfully");
                        viewModel.saveEditsCommand.execute();
                    }

                    function errorCallback(xhr) {
                        viewModel.saveCommand.indicateFailure();
                        viewModel.saveCommand.pushError("Inventory Pick Order Failed to Save. " + xhr.responseText);
                    }
                },
                canExecute: function(isExecuting) {
                    return !isExecuting;
                },
                log_name: 'InventoryPickOrderViewModel.save'
            });
            viewModel.removeItemCommand = ko.command({
                execute: function(item) {
                    removeItem.call(viewModel, item);
                }
            });


            // subscribers
            viewModel.newItems.subscribe(function(newVal) {
                addEmptyItem.apply(viewModel, [newVal]);
            });

            //initialize
            if (viewModel.enablePicking()) {
                loadChileProductOptions('WIP');
                loadPackagingProductOptions();

                addEmptyItem.apply(viewModel);
            }

            ajaxStatusHelper.init(viewModel.saveCommand);
        }
        
        target.InventoryPickOrderViewModel = viewModel || null;
        return viewModel;
    };

    var cacheMapping = {
        "PickOrderItems": function (data) {
            return ko.utils.arrayMap(data, function(item) {
                var pickOrderItem = new InventoryPickOrderItem(item);
                delete pickOrderItem.__ko_mapping__;
                return pickOrderItem;
            });
        },
    };
    
    return self;
    
    //#region private members
    
    function initNewItem() {
        var newItem = new InventoryPickOrderItem();
        newItem.empty = ko.computed(function() {
            return !this.Product()
                && !this.TreatmentKey()
                && !this.Packaging()
                && !this.Quantity();
        }, newItem);

        // This property allows the items to maintain a reference to the property dropdown options -VK
        newItem.productOptions = ko.computed(function () {
            return self.productOptionsByType()[this.ProductType()] || [];
        }, newItem);

        // subscribers
        newItem.ProductType.subscribe(function (ptype) {
            self.productTypeFilter(ptype);
        });
        newItem.Product.subscribe(function (newVal) {
            if (newVal) addEmptyItem.apply(this);
        }, this);

        return newItem;
    }
    function addEmptyItem(items) {
        items = items || ko.utils.unwrapObservable(this.newItems);
        if (!this.enablePicking()) return;
        var lastItem = items[items.length - 1];
        if (!lastItem || needsNewEmptyItem()) {
            var empty = initNewItem.apply(this);
            this.newItems.push(empty);
        };


        function needsNewEmptyItem() {
            return lastItem.empty ? 
                !lastItem.empty()
                :true;
        }
    }
    function removeItem(item) {
        if (ko.utils.arrayIndexOf(this.PickOrderItems(), item) >= 0) {
            ko.utils.arrayRemoveItem(this.PickOrderItems(), item);
            this.PickOrderItems.notifySubscribers(this.PickOrderItems());
        } else {
            ko.utils.arrayRemoveItem(this.newItems(), item);
            this.newItems.notifySubscribers(this.newItems());
        }
    }
    function saveNewItems() {
        if (this.itemsToAdd().length > 0) {
            ko.utils.arrayPushAll(this.PickOrderItems(), this.itemsToAdd());
            this.clearNewItems();
            this.PickOrderItems.notifySubscribers(this.PickOrderItems());
        }
    }
    function clearNewItems() {
        this.newItems([]);
    }
    function loadChileProductOptions(chileState, options) {
        options = options || {};
        api.products.getChileProducts(chileState, {
            completeCallback: options.completedCallback,
            failCallback: function() {
                showUserMessage("Chile products failed to load after multiple attempts. There may be problem with the server.");
            },
            successCallback: function(resultData) {
                var mapped = ko.utils.arrayMap(resultData, function (item) {
                    return {
                        ProductKey: item.ProductKey,
                        ProductName: item.ProductFullName,
                    };
                });
                self.productOptions(mapped);
            }
        });
        return;
    }
    function loadPackagingProductOptions(options) {
        options = options || {};
        api.products.getPackagingProducts({
            completeCallback: options.completedCallback,
            failCallback: function() {
                showUserMessage("Chile products failed to load after multiple attempts. There may be a problem with the server.");
                if (options.failCallback) options.failCallback();
            },
            successCallback: function(data) {
                self.packagingProductOptions(data);
                if (options.successCallback) options.successCallback();
            }
        });
    }
    function savePickOrder(pickOrderKey, pickOrderType, dto, options) {
        var data = ko.toJSON(dto);
        $.ajax({
            url: "/api/" + pickOrderType.displayValue() + "/pickOrders/" + pickOrderKey,
            type: 'PUT',
            contentType: 'application/json',
            data: data,
            success: options.successCallback,
            error: options.errorCallback,
            complete: options.completeCallback,
        });
    }
    function getPickOrderDto() {
        var model = this;

        var orderItems = ko.utils.arrayFilter(model.allItems(), function (item) {
            return item && (item.empty ? !item.empty() : true);
        });

        var dto = {
            InventoryPickOrderItems: ko.utils.arrayMap(orderItems, pickOrderItemDto),
            OrderKey: ko.utils.unwrapObservable(model.InventoryPickKey) || "",
        };

        return dto;
        
        function pickOrderItemDto(item) {
            return {
                ProductKey: ko.utils.unwrapObservable(item.ProductKey),
                PackagingKey: ko.utils.unwrapObservable(item.PackagingKey),
                TreatmentKey: ko.utils.unwrapObservable(item.TreatmentKey),
                Quantity: ko.utils.unwrapObservable(item.Quantity),
            };
        }
    }
    function mapPickOrderItem(data) {
        if (!data.Packaging) {
            data.Packaging = {
                ProductKey: data.PackagingProductKey,
                ProductName: data.PackagingName,
                Weight: data.PackagingWeight,
            };
            delete data.PackagingName;
            delete data.PackagingWeight;
        }
        if (!data.Product) {
            data.Product = {
                ProductKey: data.ProductKey,
                ProductName: data.ProductKey + ' ' + data.ProductName,
            };
            delete data.ProductKey;
            delete data.ProductName;
        }
        return new InventoryPickOrderItem(data);
    }
    
    //#endregion
    
}(ko, ajaxStatusHelper, EsmHelper));

function InventoryPickOrderItem(data) {
    var model = {
        Product: ko.observable(),
        Packaging: ko.observable(),
    };

    var defaultProps = {
        ProductType: null,
        TreatmentKey: null,
        Quantity: null,
    };
    var input = $.extend({}, defaultProps, data);
    ko.mapping.fromJS(input, InventoryPickOrderItem.mapping, model);


    // computed properties
    model.productTypeFilter = model.ProductType;
    model.ProductName = ko.computed(function () {
        return this() ? ko.utils.unwrapObservable(this().ProductName) : undefined;
    }, model.Product);
    model.PackagingName = ko.computed(function () {
        return this() ? ko.utils.unwrapObservable(this().ProductName) : undefined;
    }, model.Packaging);
    model.PackagingWeight = ko.computed(function () {
        return this() ? ko.utils.unwrapObservable(this().Weight) : '';
    }, model.Packaging);
    model.TotalWeight = ko.computed(function () {
        return ko.utils.unwrapObservable(model.Quantity) * model.PackagingWeight();
    }, self);

    // key properties
    model.ProductKey = ko.computed(function () {
        return this.Product() ? this.Product().ProductKey : undefined;
    }, model);
    model.PackagingKey = ko.computed(function () {
        return this.Packaging() ? this.Packaging().ProductKey : undefined;
    }, model);

    return model;
    
};

InventoryPickOrderItem.mapping = {
    'Quantity': {
        create: function(options) {
            return ko.numericObservable(options.data).extend({
                min: 1
            });
        }
    },
    'TreatmentKey': {
        create: function(options) {
            return ko.observable(options.data).extend({ treatmentType: true });
        }
    },
    'ProductType': {
        create: function(options) {
            return ko.observable(options.data).extend({ productType: true });
        }
    },
    ignore: ['__ko_mapping__']
};