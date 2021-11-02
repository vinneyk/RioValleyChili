ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
ko.components.register('inter-warehouse-movement-editor', require('App/components/warehouse/inter-warehouse-movement-editor/inter-warehouse-movement-editor'));
ko.components.register('inter-warehouse-movement-summary', require('App/components/warehouse/inter-warehouse-movement-summary/inter-warehouse-movement-summary'));
ko.components.register('post-and-close-inventory-order', require('App/components/warehouse/post-and-close-inventory-order/post-and-close-inventory-order'));
ko.components.register('inventory-picker', require('App/components/inventory/inventory-picker/inventory-picker'));
ko.components.register('inventory-filters', require('App/components/common/lot-filters/lot-filters'));


var rvc = require('app'),
    warehouseService = require('services/warehouseService'),
    warehouseLocationsService = require('services/warehouseLocationsService'),
    page = require('page');

require('App/koExtensions');

ko.bindingHandlers.onEnter = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var allBindings = allBindingsAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                allBindings.onEnter.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

function InterWarehouseMovementsViewModel() {
    if (!(this instanceof InterWarehouseMovementsViewModel)) return new InterWarehouseMovementsViewModel();

    var self = this,
    inventoryPickerData = ko.observable();

    // Methods and properties to dispose via .dispose()
    self.disposables = [];

    // Initial bindings
    self.currentMovement = ko.observable();
    self.warehouses = ko.observableArray();
    self.destinationFacilityLocations = ko.observableArray();
    self.searchBoxValue = ko.observable();
    self.searchKey = ko.observable();
    self.filtersInput = ko.observable();

    self.editorExports = ko.observable();
    self.summaryExports = ko.observable();
    self.inventoryPickerExports = ko.computed({
        read: function () {
            return inventoryPickerData();
        },
        write: function (data) {
            inventoryPickerData(data());
        }
    });
    self.postAndCloseExports = ko.observable();
    self.filtersExports = ko.observable();

    self.isNewMovement = ko.computed(function () {
        var val = self.currentMovement();
        return (!val || !ko.unwrap(val.MovementKey));
    });
    self.isPostAndCloseShowing = ko.observable(false);
    self.isPickerShowing = ko.observable(false);

    self.filters = {
        originFacilityKeyFilter: ko.observable(),
        destinationFacilityKeyFilter: ko.observable(),
        beginningShipmentDateFilter: ko.observableDate(),
        endingShipmentDateFilter: ko.observableDate(),
        orderStatusFilter: ko.observable(),
        shipmentStatusFilter: ko.observable().extend({ shipmentStatusType: true })
    };


    self.isWorking = ko.observable(false);
    self.loadingMessage = ko.observable('');

    self.movementKey = ko.observable();
    self.currentEditorItem = ko.computed(function () {
        var editor = self.editorExports();
        var movement = editor && editor.currentMovement();
        // creating self.movementKey as a computed observable
        // was resulting in an unexpected behavior in which
        // the value stopped being updated after loading and closing
        // a treatment order -- VK 9/25/2015
        self.movementKey(movement ? movement.MovementKey() : null);
        return movement;
    });

    warehouseService.getWarehouses()
        .then(function (values) {
            self.warehouses(values);
    });

    self.movementFacility = ko.pureComputed(function () {
        var movement = self.currentMovement();
        return  movement &&
                movement.DestinationFacilityDetails() &&
                movement.DestinationFacilityDetails().FacilityName;
    });

    self.disposables.push(ko.computed(function () {
        if (self.summaryExports() && self.summaryExports().selectedMovement()) {
            var key = self.summaryExports().selectedMovement().MovementKey;
            if (key !== self.movementKey()) self.setSearchKey(key);
        }
    }));

    self.updateItem = function (key) {
        var dfd = $.Deferred();

        warehouseService.getInterWarehouseDetails(key)
            .done(function (data) {
                self.summaryExports().updateSummaryItem(key, data);
                self.currentMovement(data);
            })
            .always(function () {
                dfd.resolve();
            });

        return dfd;
    };

    self.loadInventoryCommand = ko.asyncCommand({
        execute: function (complete) {
            var picker = self.inventoryPickerExports();
            return picker.loadInventoryItemsCommand.execute()
                .always(complete);
        },
        canExecute: function (isExecuting) {
            if (isExecuting) return;
            var picker = self.inventoryPickerExports();
            return picker && picker.loadInventoryItemsCommand.canExecute();
        }
    });

    self.isFiltersShowing = ko.pureComputed(function () {
        return self.isPickerLoaded() && !self.isPostAndCloseShowing();
    });
    self.isEditorControlsShowing = ko.pureComputed(function () {
        return !self.isPickerShowing() && !self.isPostAndCloseShowing();
    });
    self.displayReportLinks = ko.pureComputed(function () {
        return self.currentEditorItem() && !self.isPickerShowing() && !self.isPostAndCloseShowing();
    });

    self.isPickerLoaded = ko.computed(function () {
        var picker = self.inventoryPickerExports();
        return picker && picker.isInit() && self.isPickerShowing();
    });

    // Bundled data
    self.inventoryPickerInputs = {
        args: ko.observable( {} ),
        pickedInventoryItems: ko.pureComputed(function () {
            var currentMovement = self.currentMovement(); //raw data
            return currentMovement && currentMovement.PickedInventory && currentMovement.PickedInventory.PickedInventoryItems;
        }),
        pickingContext: rvc.lists.inventoryPickingContexts.TransWarehouseMovements,
        pickingContextKey: self.movementKey,
        filters: self.filtersExports,
        pageSize: 50,
        targetProduct: ko.observable(  ),
        targetWeight: ko.observable(  ),
        customerLotCode: ko.observable(),
        customerProductCode: ko.observable()
    };


    // Behaviors
    self.createNewMovement = ko.command({
        execute: function () {
            page('/new');
        },
        canExecute: function () {
            return true;
        }
    });

    var canMovementBePosted = ko.computed(function () {
        if (!(self.postAndCloseExports() && !self.isNewMovement())) { return false; }

        var editor = self.editorExports(),
            movement = editor && editor.currentMovement(),
            shipment = movement && movement.Shipment(),
            pickedInventory = movement && movement.PickedInventory() || [];

        return editor && shipment && pickedInventory
            && !editor.isLocked()
            && pickedInventory.length
            && shipment.Status < rvc.lists.shipmentStatus.Shipped.key;
    });

    self.showPostAndCloseCommand = ko.command({
        execute: function () {
            var key = self.currentEditorItem().DestinationFacilityKey();
            self.getWarehouseLocationsByFacilityAsync(key)
                .done(function (data) {
                    self.destinationFacilityLocations(data);
                    self.isPostAndCloseShowing(true);
                })
                .fail(function (xhr, status, message) {
                    showUserMessage("Failed to load locations for destination facility.", {
                        description: 'Error: "' + message + '"'
                    });
                });
        },
        canExecute: function () {
            return canMovementBePosted();
        }
    });
    self.postAndCloseCommand = ko.asyncCommand({
        execute: function (complete) {
            self.postAndCloseExports().postAndCloseAsync()
                .done(function() {
                    showUserMessage("Post & close successful", { description: "Inventory has been added to the specified destinations." });
                    self.updateItem(self.movementKey());
                    self.cancelPostAndCloseCommand.execute();
                }).fail(function (xhr, status, message) {
                    showUserMessage("Failed to post treatment order.", { description: 'Error: "' + message + '"' });
                }).always(function () {
                    complete();
                    self.isWorking(false);
                });;
        },
        canExecute: function () {
            return canMovementBePosted();
        }
    });
    self.cancelPostAndCloseCommand = ko.command({
        execute: function () {
            self.isPostAndCloseShowing(false);
        }
    });
    self.searchForKey = function () {
        var targetKey = self.searchBoxValue();

        if (targetKey) {
            page('/' + targetKey);
        }
    };
    self.setSearchKey = function (key) {
        if (key === undefined) {
            page('/');
            return;
        }

        if (self.summaryExports() && self.summaryExports().selectedMovement()) {
            self.summaryExports().selectedMovement(undefined);
        }

        self.searchKey(key);
        self.isWorking(key ? true : false);

        if (currentMovementKey != key) {
            page('/' + (key || ''));
        }
    };
    self.loadInventoryCommand = ko.command({
        execute: function() {
            self.inventoryPickerExports().loadInventoryItemsCommand.execute();
        },
        canExecute: function() {
            return self.inventoryPickerExports() && self.inventoryPickerExports().loadInventoryItemsCommand.canExecute();
        }
    });

    self.disposables.push(self.movementKey, self.isWorking);

    self.saveCommand = ko.asyncCommand({
        execute: function(complete) {
            var key = self.isNewMovement() ? null : self.movementKey();
            if (!self.editorExports().currentMovement.isValid()) {
                showUserMessage('Validation Errors', { description: 'The Movement contains validation errors. Please correct the errors and retry.' });
                complete();
                return;
            }

            self.isWorking(true);
            self.loadingMessage('Saving Movement...');

            var values = self.editorExports().currentMovement.asDto(),
                isNew = self.isNewMovement(),
                dfd = isNew
                    ? warehouseService.createInterWarehouseMovement(values)
                    : warehouseService.updateInterWarehouseMovement(key, values);

            dfd.then(function(data) {
                    var movementKey = isNew ? data : key;
                    showUserMessage("Save Successful", {
                        description: 'Movement ' + movementKey + ' has been ' +
                        (isNew ? 'created' : 'updated') + '.'
                    });
                    if (isNew) {
                        self.summaryExports().addSummaryItem(movementKey);
                        page('/' + data);
                    } else {
                        self.updateItem(movementKey);
                    }
                })
                .fail(function(xhr, status, message) {
                    showUserMessage('Failed to save movement', {
                        description: message
                    });
                })
                .always(function() {
                    complete();
                    self.isWorking(false);
                });

            self.isPostAndCloseShowing(false);
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.currentEditorItem() && !self.editorExports().isLocked();
        }
    });

    self.cancelCommand = ko.command({
        execute: function () {
            page('/');
        },
        canExecute: function () {
            return self.currentEditorItem() && true;
        }
    });

    self.pickForItemCommand = ko.command({
        execute: function(targetProduct) {
            var input = targetProduct || {};
            self.isPickerShowing(true);

            var filters = self.filtersExports();
            filters.inventoryType(input.filters.inventoryType);
            filters.lotType(input.filters.lotType);
            filters.productKey(input.filters.productKey);
            filters.treatmentKey(input.filters.treatmentKey);
            filters.packagingProductKey(input.filters.packagingProductKey);

            self.inventoryPickerInputs.targetProduct( targetProduct.targetProduct );
            self.inventoryPickerExports().loadInventoryItemsCommand.execute();
            self.inventoryPickerInputs.targetWeight( targetProduct.targetWeight );
            //NOTE: WH Transfers are not currently able to maintain their orderItemKey due to limitations in Access data models.
            //      When the web system is decoupled form Access, this functionality can be restored (pending a refactoring of the association in the web sys models).
            //self.inventoryPickerInputs.args( { orderItemKey: targetProduct.orderItemKey });
            //self.inventoryPickerInputs.customerLotCode(input.customerLotCode);
            //self.inventoryPickerInputs.customerProductCode(input.customerProductCode);
        },
        canExecute: function() {
            var editor = self.editorExports(),
                        movement = editor && editor.currentMovement(),
                        shipment = movement && movement.Shipment();
            return shipment && !self.isNewMovement()
                && ko.unwrap(movement.OrderStatus) < rvc.lists.orderStatus.Fulfilled.key
                && ko.unwrap(shipment.Status) < rvc.lists.shipmentStatus.Shipped.key;
        }
    });

    function clearMovements() {
        var summary = self.summaryExports();
        if (summary && ko.isObservable(summary.selectedMovement)) {
            summary.selectedMovement(null);
        }

        self.currentMovement(null);
        self.isPostAndCloseShowing(false);
        self.isPickerShowing(false);
        self.searchBoxValue(null);
        self.searchKey(null);
    }
    self.cancelPickerCommand = ko.command({
        execute: function () {
            self.isPickerShowing(false);
            self.inventoryPickerExports().revertCommand.execute();
        },
        canExecute: function () {
            return self.inventoryPickerExports() && true;
        }
    });


    ko.postbox.subscribe('PickedItemsSaved', function (values) {
        self.isWorking(true);
        self.loadingMessage('Updating movement data...');
        self.updateItem(self.movementKey()).always(function () {
            self.isPickerShowing(false);
            self.isWorking(false);
        });
    });

    // Routing
    var currentMovementKey;

    page.base('/Warehouse/InterWarehouseMovements');
    page('/:key?', navigateToMovement);

    function navigateToMovement(ctx) {
        var transferKey = ctx.params.key;
        clearMovements();

        // No routing specified
        if (ctx.params.key == undefined) {
            currentMovementKey = undefined;
            document.title = "Movements Between Facilities";
            self.isWorking(false);

            return;
        }

        // New entry
        if (transferKey === 'new') {
            currentMovementKey = null;
            document.title = "Create New Inter-Warehouse Movement";

            var editor = self.editorExports();
            if (editor) {
                self.initNewMovementOrder();
            } else {
                var editorSubscription = self.editorExports.subscribe(function () {
                    self.initNewMovementOrder();
                    editorSubscription.dispose();
                    ko.utils.arrayRemoveItem(self.disposables, editorSubscription);
                });
                self.disposables.push(editorSubscription);
            }

            return;
        }

        // Existing entry
        if (currentMovementKey !== transferKey) {
            currentMovementKey = transferKey;
            self.loadInterWarehouseMovementByKey(transferKey);
            return;
        }
    }
}

InterWarehouseMovementsViewModel.prototype.initNewMovementOrder = function () {
    this.currentMovement({});
}
InterWarehouseMovementsViewModel.prototype.loadInterWarehouseMovementByKey = function (key) {
    var dfd = warehouseService.getInterWarehouseDetails(key),
        self = this;

    self.isWorking(true);
    self.loadingMessage('Loading movement #' + key + '...');

    dfd.done(function (values) {
        self.currentMovement(values);
        document.title = 'Movement Order #' + key;
    }).fail(function () {
        showUserMessage("No Treatment Order Found", {
            description: "The Movement Order # " + key + " is invalid or doesn't exist. If you believe this is an error, please contact system administrators to report the issue."
        });
        self.currentMovement(null);
    }).always(function () {
        self.isWorking(false);
    });

    return dfd;
}

var facilityLocations = {};
InterWarehouseMovementsViewModel.prototype.getWarehouseLocationsByFacilityAsync = function (facilityKey) {
    var dfd = $.Deferred();
    var locations = facilityLocations[facilityKey];
    if (locations) {
        dfd.resolve(locations);
    } else {
        warehouseLocationsService.getWarehouseLocations(facilityKey)
            .done(function (values) {
                facilityLocations[facilityKey] = values;
                dfd.resolve(values);
            })
            .fail(dfd.reject);
    }
    return dfd;
}

InterWarehouseMovementsViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

InterWarehouseMovementsViewModel.prototype.disposeOne = function(propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

var vm = new InterWarehouseMovementsViewModel();

ko.applyBindings(vm);
ko.punches.enableAll();

page();
